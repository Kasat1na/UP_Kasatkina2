using DynamicData.Kernel;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteErasee.Models;

namespace WriteErasee.ViewModels
{
	public class OrderUserViewModel : ViewModelBase
	{
		public double FullCost { get => _fullCost; set => this.RaiseAndSetIfChanged(ref _fullCost, value); }
		private double _fullCost = 0;

		public double FullDiscount { get => _fullDiscount; set => this.RaiseAndSetIfChanged(ref _fullDiscount, value); }
		private double _fullDiscount = 0;

		public string CurrentUserFullName { get => _currentUserFullName; set => this.RaiseAndSetIfChanged(ref _currentUserFullName, value); }
		private string _currentUserFullName = "Гость";
		User? _currentUser;

		public Order Order { get => _order; set => this.RaiseAndSetIfChanged(ref _order, value); }
		Order _order;

		public List<OrdersProduct>? OrdersProducts { get => _ordersProducts; set => this.RaiseAndSetIfChanged(ref _ordersProducts, value); }	
		List<OrdersProduct>? _ordersProducts;

		public List<PickUpPoint> PickUpPoint { get => _pickUpPoint; set => this.RaiseAndSetIfChanged(ref _pickUpPoint, value); }
		List<PickUpPoint> _pickUpPoint;

		public OrderUserViewModel(User? user, List<OrdersProduct> ordersProducts)
		{
			_currentUser = user;
			if (user != null) CurrentUserFullName = user.Surname + " " + user.Name + " " + user.Patronymic;
			OrdersProducts = ordersProducts;
			PickUpPoint = MainWindowViewModel.myConnection.PickUpPoints.ToList();
			Order = MainWindowViewModel.myConnection.Orders.Include(it => it.IdPickUpPointNavigation)
				.First(it => it.Id == ordersProducts[0].OrderId);
			UpdateCost();
		}
		
		private void UpdateCost()
		{
			FullCost = 0;
			FullDiscount = 0;
			double cost = 0;
			if (OrdersProducts != null)
			{
				OrdersProducts.ForEach(it =>
				{
					double costProd = it.ProductArticleNumberNavigation.Cost;
					double currentDiscount = (double)it.ProductArticleNumberNavigation.CurrentDiscount;
					cost += (it.Count * costProd);
					double costOneProd = (costProd / 100) * (100 - currentDiscount);
					FullCost += it.Count * costOneProd;
				});
				if (cost > 0) FullDiscount = (1.0 - (FullCost / cost)) * 100;
			}
		}

		public void AddUnit(OrdersProduct product)
		{
			foreach (OrdersProduct item in OrdersProducts)
				if (item.ProductArticleNumber == product.ProductArticleNumber)
					item.Count++;			
			OrdersProducts = new List<OrdersProduct>(OrdersProducts);
			UpdateCost();
		}

		public void DeleteUnit(OrdersProduct product)
		{
			if (product.Count == 1)
			{
				if (OrdersProducts.Count == 1) OrdersProducts = null;
				else
				{
					OrdersProducts.Remove(product);
					OrdersProducts = new List<OrdersProduct>(OrdersProducts);
				}
			}
			else
			{
				foreach (OrdersProduct item in OrdersProducts)
					if (item.ProductArticleNumber == product.ProductArticleNumber)
						item.Count--;
				OrdersProducts = new List<OrdersProduct>(OrdersProducts);
			}
			UpdateCost();
		}

		/// <summary>
		/// Создание заказа
		/// </summary>
		public void SaveOrder()
		{
			try
			{
				int count = 0;
				foreach (var item in OrdersProducts)
				{
					if (item.ProductArticleNumberNavigation.QuantityInStock > 3) count++;
				}
				DateOnly dateOrder = new DateOnly(Order.DateOrder!.Value.Year,
					Order.DateOrder!.Value.Month, Order.DateOrder!.Value.Day);
				//срок доставки – 3 дня
				if (OrdersProducts.Count == count) Order.DateDelivery = dateOrder.AddDays(3);
				//иначе 6 дней
				else Order.DateDelivery = dateOrder.AddDays(6);
				Random rnd = new Random();
				//Код генерируется случайным образом (100-999)
				Order.Code = rnd.Next(100, 1000);
				MainWindowViewModel.myConnection.Orders.Update(Order);
				MainWindowViewModel.myConnection.OrdersProducts.AddRange(OrdersProducts);
				MainWindowViewModel.myConnection.SaveChanges();
				Exit();
			}
			catch
			{
				Exit();
			}
		}

		public void Exit()
		{
			if (_currentUser != null)
				MainWindowViewModel.Instance.PageContent = new Show { DataContext = new ShowViewModel(_currentUser) };
			else
				MainWindowViewModel.Instance.PageContent = new Show { DataContext = new ShowViewModel() };
		}
	}
}
