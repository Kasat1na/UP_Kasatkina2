using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteErasee.Models;
using ReactiveUI;
using Microsoft.EntityFrameworkCore;

namespace WriteErasee.ViewModels
{
	public class EditProductViewModel : ViewModelBase
	{
		private User? _currentUser;
		public bool IsNewProduct { get => _isNewProduct; set => this.RaiseAndSetIfChanged(ref _isNewProduct, value); }
		private bool _isNewProduct = true;
		public string Title { get => _title; set => this.RaiseAndSetIfChanged(ref _title, value); }
		private string _title = "Добавление товара";
		public Product Product { get => _product; set => this.RaiseAndSetIfChanged(ref _product, value); }
		private Product _product = new();
		public List<Category> Categories { get => _categories; set => this.RaiseAndSetIfChanged(ref _categories, value); }
		private List<Category> _categories = new();
		public List<Manufacturer> Manufacturers { get => _manufacturers; set => this.RaiseAndSetIfChanged(ref _manufacturers, value); }
		private List<Manufacturer> _manufacturers = new();
		public List<Supplier> Suppliers { get => _suppliers; set => this.RaiseAndSetIfChanged(ref _suppliers, value); }
		private List<Supplier> _suppliers = new();
		public List<Unit> Units { get => _units; set => this.RaiseAndSetIfChanged(ref _units, value); }
		private List<Unit> _units = new();
		private void Launch()
		{
			Categories = MainWindowViewModel.myConnection.Categories.ToList();
			Manufacturers = MainWindowViewModel.myConnection.Manufacturers.ToList();
			Suppliers = MainWindowViewModel.myConnection.Suppliers.ToList();
			Units = MainWindowViewModel.myConnection.Units.ToList();
		}
		public EditProductViewModel(User? user)
		{
			_currentUser = user;
			Launch();
		}
		public EditProductViewModel(User? user, string article)
		{
			Title = "Редактирование товара";
			IsNewProduct = false;
			_currentUser = user;
			Product = MainWindowViewModel.myConnection.Products
				.Include(it => it.IdCategoryNavigation)
				.Include(it => it.IdManufacturerNavigation)
				.Include(it => it.IdSupplierNavigation)
				.Include(it => it.IdUnitNavigation)
				.First(it => it.ArticleNumber == article);
			Launch();
		}
		public async Task Save()
		{
			try
			{
				if (IsNewProduct)
				{
					MainWindowViewModel.myConnection.Products.Add(Product);
					MainWindowViewModel.myConnection.SaveChanges();
					await MessageBoxManager.GetMessageBoxStandard("Добавление", "Товар успешно добавлен", ButtonEnum.Ok).ShowAsync();
				}
				else
				{
					MainWindowViewModel.myConnection.Products.Update(Product);
					MainWindowViewModel.myConnection.SaveChanges();
					await MessageBoxManager.GetMessageBoxStandard("Редактирование", "Данные о товаре изменены", ButtonEnum.Ok).ShowAsync();
				}
				Exit();
			}
			catch
			{
				await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Проверьте правильность заполнения полей и попробуйте еще раз", ButtonEnum.Ok).ShowAsync();
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
