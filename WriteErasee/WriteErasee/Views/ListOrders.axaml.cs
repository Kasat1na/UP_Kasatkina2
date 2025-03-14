using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WriteErasee.Models;
using WriteErasee.ViewModels;

namespace WriteErasee;

public partial class ListOrders : UserControl
{
    public ListOrders()
    {
        InitializeComponent();
	}

	public ListOrders(User? user)
	{
		InitializeComponent();
		DataContext = new ListOrdersViewModel(user);
	}
}