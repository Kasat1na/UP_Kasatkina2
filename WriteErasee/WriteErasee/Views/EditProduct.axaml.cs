using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WriteErasee.Models;
using WriteErasee.ViewModels;

namespace WriteErasee;

public partial class EditProduct : UserControl
{
    public EditProduct()
    {
        InitializeComponent();
    }

	public EditProduct(User? user)
	{
		InitializeComponent();
		DataContext = new EditProductViewModel(user);
	}

	public EditProduct(User? user, string article)
	{
		InitializeComponent();
		DataContext = new EditProductViewModel(user, article);
	}
}