using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WriteErasee.Models;
using WriteErasee.ViewModels;

namespace WriteErasee;

public partial class EditOrder : UserControl
{
    public EditOrder()
    {
        InitializeComponent();
    }
	public EditOrder(User? user, Order order)
	{
		InitializeComponent();
		DataContext = new EditOrderViewModel(user, order);
	}
}