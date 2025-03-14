using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WriteErasee.Models;

namespace WriteErasee.ViewModels
{
	public class ListOrdersViewModel : ViewModelBase
	{
		User? _currentUser;
		public int SelectedSortedType
		{
			get => _selectedSortedType;
			set
			{
				this.RaiseAndSetIfChanged(ref _selectedSortedType, value);
				Filter();
			}
		}
		private int _selectedSortedType = 0;

		public int SelectedFilterType
		{
			get => _selectedFilterType;
			set
			{
				this.RaiseAndSetIfChanged(ref _selectedFilterType, value);
				Filter();
			}
		}
		private int _selectedFilterType = 0;

		public List<string> ListSortedType { get => _listSortedType; set => this.RaiseAndSetIfChanged(ref _listSortedType, value); }
		private List<string> _listSortedType = new()
		{
			"Без сортировки",
			"По возрастанию стоимости",
			"По убыванию стоимости",
		};
		public List<string> ListFilterType { get => _listFilterType; set => this.RaiseAndSetIfChanged(ref _listFilterType, value); }
		private List<string> _listFilterType = new()
		{
			"Все диапазоны",
			"от 0 до 10%",
			"от 11 до 14%",
			"15% и более",
		};

		public List<Order> Orders { get => _orders; set => this.RaiseAndSetIfChanged(ref _orders, value); }
		private List<Order> _orders = new();
		public List<Order> OrdersPreview { get => _ordersPreview; set => this.RaiseAndSetIfChanged(ref _ordersPreview, value); }
		private List<Order> _ordersPreview = new();
		private void Filter()
		{
			OrdersPreview = Orders;
			if (_selectedSortedType != 0)
			{
				if (_selectedSortedType == 1) OrdersPreview = OrdersPreview.OrderBy(it => it.Cost).ToList();
				if (_selectedSortedType == 2) OrdersPreview = OrdersPreview.OrderByDescending(it => it.Cost).ToList();
			}
			if (_selectedFilterType != 0)
			{
				if (_selectedFilterType == 2) OrdersPreview = OrdersPreview
						.Where(it => it.Discount >= 0 && it.Discount <= 10).ToList();
				if (_selectedFilterType == 2) OrdersPreview = OrdersPreview
						.Where(it => it.Discount >= 11 && it.Discount <= 14).ToList();
				if (_selectedFilterType == 2) OrdersPreview = OrdersPreview.Where(it => it.Discount >= 15).ToList();
			}
		}
		public ListOrdersViewModel(User? user)
		{
			_currentUser = user;
			Orders = MainWindowViewModel.myConnection.Orders
				.Include(it => it.IdClientNavigation)
				.Include(it => it.IdPickUpPointNavigation)
				.Include(it => it.IdStatusNavigation)
				.ToList();
			Orders.ForEach(it => {
				it.FIO = (it.IdClientNavigation == null) ? "Гость" :
				(it.IdClientNavigation.Surname + " " + it.IdClientNavigation.Name + " " + it.IdClientNavigation.Patronymic );
				// Цвет по умолчанию
				it.Color = "#ffffff";
				int count = 0;
				double cost = 0;
				List<OrdersProduct> ordersProducts = MainWindowViewModel.myConnection.OrdersProducts
				.Where(op => op.OrderId == it.Id).Include(it => it.ProductArticleNumberNavigation).ToList();
				foreach (OrdersProduct product in ordersProducts)
				{
					double costProd = product.ProductArticleNumberNavigation.Cost;
					double currentDiscount = (double)product.ProductArticleNumberNavigation.CurrentDiscount;
					cost += (product.Count * costProd);
					double costOneProd = (costProd / 100) * (100 - currentDiscount);
					it.Cost += product.Count * costOneProd;
					if (cost > 0)
						it.Discount = (1.0 - (it.Cost / cost)) * 100;
					if (product.ProductArticleNumberNavigation.QuantityInStock > 3)
						count++;
					
					else if (product.ProductArticleNumberNavigation.QuantityInStock == 0)
						it.Color = "#ff8c00"; // Xотя бы одного товара нет на складе
				}
				//Все товары в заказе есть на складе в наличии более 3 позиций
				if (it.Color == "#ffffff" && ordersProducts.Count == count) it.Color = "#20b2aa";
			});
			Filter();
		}
		public void Exit()
		{
			if (_currentUser != null)
				MainWindowViewModel.Instance.PageContent = new Show { DataContext = new ShowViewModel(_currentUser) };
			else
				MainWindowViewModel.Instance.PageContent = new Show { DataContext = new ShowViewModel() };
		}

		public Order SelectedOrder
		{
			get => _selectedOrder;
			set
			{
				this.RaiseAndSetIfChanged(ref _selectedOrder, value);
				MainWindowViewModel.Instance.PageContent = new EditOrder(_currentUser, SelectedOrder);
			}
		}
		private Order _selectedOrder;
	}
}
