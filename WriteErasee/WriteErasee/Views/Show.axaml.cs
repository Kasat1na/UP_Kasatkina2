using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WriteErasee.ViewModels;

namespace WriteErasee;

public partial class Show : UserControl
{
    public Show()
    {
        InitializeComponent();
        this.DataContext = new ShowViewModel();
    }
}