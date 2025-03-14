using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ReactiveUI;
using WriteErasee.Models;
using Avalonia.Controls;

namespace WriteErasee.ViewModels
{
	public class ShowViewModel : ViewModelBase
    {
        private List<Product> _productList;
        public List<Product> ProductList
        {
            get => _productList;
            set => this.RaiseAndSetIfChanged(ref _productList, value);
        }
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                this.RaiseAndSetIfChanged(ref _searchQuery, value);
                ApplySearchFilter();

            }
        }
        private int _totalProductCount;
        public int TotalProductCount
        {
            get => _totalProductCount;
            set => this.RaiseAndSetIfChanged(ref _totalProductCount, value);
        }
        private int _filteredProductCount;
        public int FilteredProductCount
        {
            get => _filteredProductCount;
            set => this.RaiseAndSetIfChanged(ref _filteredProductCount, value);
        }
        private User? _currentUser;
        public string CurrentUserFullName => _currentUser != null
            ? $"{_currentUser.Surname} {_currentUser.Name} {_currentUser.Patronymic}"
            : "Гость";
        private bool _canViewOrder;
        public bool CanViewOrder
        {
            get => _canViewOrder;
            set => this.RaiseAndSetIfChanged(ref _canViewOrder, value);
        }
		public Product SelectedProduct
		{
			get => _selectedProduct;
			set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
		}
		private Product _selectedProduct;
		public ReactiveCommand<Product, Unit> AddToOrderCommand { get; }
        public ShowViewModel(User? user = null)
        {
            _currentUser = user;
            Load();
            CanViewOrder = false;
        }
        private void Load()
        {
            ProductList = MainWindowViewModel.myConnection.Products.Include(x => x.OrdersProducts).ToList();
            if (_currentUser != null)
            {
                var roleName = _currentUser.Role?.Name;
                if (roleName == "Менеджер")
                {
					IsVisibleManager = true;
                }
                else if (roleName == "Администратор")
                {
					IsVisibleAdmin = true;
					IsVisibleManager = true;
                }
            }
            ApplyFilters();
            TotalProductCount = ProductList.Count;
        }
        private bool _isPriceAscending;
        public bool IsPriceAscending
        {
            get => _isPriceAscending;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isPriceAscending, value) && value)
                {
                    SortByPriceAscending();
                }
            }
        }
        private bool _isPriceDescending;
        public bool IsPriceDescending
        {
            get => _isPriceDescending;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isPriceDescending, value) && value)
                {
                    SortByPriceDescending();
                }
            }
        }
        public void SortByPriceAscending()
        {
            ProductList = ProductList.OrderBy(p => p.Cost).ToList();
            this.RaisePropertyChanged(nameof(ProductList));
        }
        public void SortByPriceDescending()
        {
            ProductList = ProductList.OrderByDescending(p => p.Cost).ToList();
            this.RaisePropertyChanged(nameof(ProductList));
        }
        private string _selectedDiscountRange = "Все диапазоны";
        public string SelectedDiscountRange
        {
            get => _selectedDiscountRange;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDiscountRange, value);
                ApplyDiscountFilter();
            }
        }
        public List<string> DiscountRanges { get; } = new List<string>
    {
        "Все диапазоны",
        "0-9,99%",
        "10-14,99%",
        "15% и более"
    };
        public void ApplyDiscountFilter()
        {
            if (SelectedDiscountRange == "Все диапазоны")
            {
                Load();
            }
            else
            {
                switch (SelectedDiscountRange)
                {
                    case "0-9,99%":
                        ProductList = ProductList.Where(p => p.CurrentDiscount >= 0 && p.CurrentDiscount < 10).ToList();
                        break;
                    case "10-14,99%":
                        ProductList = ProductList.Where(p => p.CurrentDiscount >= 10 && p.CurrentDiscount < 15).ToList();
                        break;
                    case "15% и более":
                        ProductList = ProductList.Where(p => p.CurrentDiscount >= 15).ToList();
                        break;
                }
            }

            this.RaisePropertyChanged(nameof(ProductList)); // Уведомляем об изменении
        }
        public void ApplyFilters()
        {
            // Загружаем все товары, фильтруем их по скидкам на уровне базы данных
            var filteredProducts = MainWindowViewModel.myConnection.Products.AsQueryable();

            // Фильтрация по диапазону скидок на сервере
            if (SelectedDiscountRange != "Все диапазоны")
            {
                switch (SelectedDiscountRange)
                {
                    case "0-9,99%":
                        filteredProducts = filteredProducts.Where(p => p.CurrentDiscount >= 0 && p.CurrentDiscount < 10);
                        break;
                    case "10-14,99%":
                        filteredProducts = filteredProducts.Where(p => p.CurrentDiscount >= 10 && p.CurrentDiscount < 15);
                        break;
                    case "15% и более":
                        filteredProducts = filteredProducts.Where(p => p.CurrentDiscount >= 15);
                        break;
                }
            }

            // Теперь делаем выборку в памяти, если нужно
            var productsListInMemory = filteredProducts.ToList(); // Загружаем в память

            // Фильтрация по поисковому запросу на стороне клиента
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                productsListInMemory = productsListInMemory
                    .Where(p => p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            if (IsPriceAscending)
            {
                productsListInMemory = productsListInMemory.OrderBy(p => p.Cost).ToList();
            }
            else if (IsPriceDescending)
            {
                productsListInMemory = productsListInMemory.OrderByDescending(p => p.Cost).ToList();
            }
            TotalProductCount = filteredProducts.Count();  // Общее количество товаров
            FilteredProductCount = productsListInMemory.Count;  // Отфильтрованные товары

            // Применяем результат фильтрации и сортировки
            ProductList = productsListInMemory;
            this.RaisePropertyChanged(nameof(ProductList)); // Уведомляем об изменении
        }
        public void ApplySearchFilter()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                // Если поисковый запрос пустой, просто загружаем все товары
                Load();
            }
            else
            {
                // Фильтруем товары по наименованию уже в памяти, после загрузки
                ProductList = ProductList.Where(p => p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
                this.RaisePropertyChanged(nameof(ProductList));  // Уведомляем об изменении
            }
        }
        public void Exit()
        {
            MainWindowViewModel.Instance.PageContent = new Authorization();
        }

        private List<OrdersProduct> _cartItems = new List<OrdersProduct>();
        public List<OrdersProduct> CartItems
        {
            get => _cartItems;
            set => this.RaiseAndSetIfChanged(ref _cartItems, value);
        }

		private Order? _order;
		private List<OrdersProduct> _ordersProducts = new();
		public bool IsVisibleCreateOrder { get => _isVisibleCreateOrder;  set => this.RaiseAndSetIfChanged(ref _isVisibleCreateOrder, value); }
		private bool _isVisibleCreateOrder = false;
		public bool IsVisibleAdmin { get => _isVisibleAdmin; set => this.RaiseAndSetIfChanged(ref _isVisibleAdmin, value); }
		private bool _isVisibleAdmin = false;
		public bool IsVisibleManager { get => _isVisibleManager; set => this.RaiseAndSetIfChanged(ref _isVisibleManager, value); }
		private bool _isVisibleManager = false;
		public void AddProductToOrder()
		{
			bool flagNoProduct = true;
			if (_order == null) CreateOrder();
			foreach (var product in _ordersProducts)
			{
				if (product.ProductArticleNumber == SelectedProduct.ArticleNumber)
				{
					product.Count++;
					flagNoProduct = false;
				}
			}
			if (flagNoProduct)
			{
				OrdersProduct ordersProduct = new OrdersProduct
				{
					OrderId = _order!.Id,
					ProductArticleNumber = SelectedProduct.ArticleNumber,
					Count = 1,
					Order = _order!,
					ProductArticleNumberNavigation =
					 MainWindowViewModel.myConnection.Products
                     .Include(it => it.IdCategoryNavigation)
					 .Include(it => it.IdManufacturerNavigation)
                     .Include(it => it.IdSupplierNavigation)
                     .Include(it => it.IdUnitNavigation)
					 .First(it => it.ArticleNumber == SelectedProduct.ArticleNumber),
				};
				_ordersProducts.Add(ordersProduct);
			}
			if (_ordersProducts.Count() > 0) IsVisibleCreateOrder = true;
		}
		private void CreateOrder()
		{
			_order = new Order();
			DateTime date = DateTime.Now;
			_order.DateOrder = new DateOnly(date.Year, date.Month, date.Day);
			_order.IdClient = (_currentUser != null) ? _currentUser.Id : null;
			_order.IdStatus = 1;
			MainWindowViewModel.myConnection.Orders.Add(_order);
			MainWindowViewModel.myConnection.SaveChanges();
		}
		public void ViewOrder() => MainWindowViewModel.Instance.PageContent = new OrderUser(_currentUser, _ordersProducts);
		public void DeleteProduct(Product product)
		{
			MainWindowViewModel.myConnection.Products.Remove(product);
			MainWindowViewModel.myConnection.SaveChanges();
			Load();
		}
		public void Edit(Product product) => MainWindowViewModel.Instance!.PageContent = new EditProduct(_currentUser, product.ArticleNumber);
		public void AddProduct() => MainWindowViewModel.Instance.PageContent = new EditProduct(_currentUser);
        public void ViewOrders() => MainWindowViewModel.Instance.PageContent = new ListOrders(_currentUser);
	}
}