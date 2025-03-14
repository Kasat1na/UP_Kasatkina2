using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using WriteErasee.Models;
using WriteErasee.ViewModels;

namespace WriteErasee;

public partial class OrderUser : UserControl
{
    public OrderUser()
    {
        InitializeComponent();
	}

	public OrderUser(User? user, List<OrdersProduct> ordersProducts)
	{
		InitializeComponent();
		DataContext = new OrderUserViewModel(user, ordersProducts);
	}
}