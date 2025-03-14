using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WriteErasee.ViewModels;

namespace WriteErasee;

public partial class Authorization : UserControl
{
    public Authorization()
    {
        InitializeComponent();
        DataContext = new AuthorizationViewModel();
    }
}