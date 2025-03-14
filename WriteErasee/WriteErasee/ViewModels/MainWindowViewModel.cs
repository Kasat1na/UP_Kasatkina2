using Avalonia.Controls;
using ReactiveUI;
using WriteErasee.Models;

namespace WriteErasee.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Instance;
        public static _43pKasatkina2upContext myConnection = new _43pKasatkina2upContext();

        private UserControl _pageContent;

        public MainWindowViewModel()
        {
            Instance = this;
            PageContent = new Authorization();
        }

        public UserControl PageContent
        {
            get => _pageContent;
            set => this.RaiseAndSetIfChanged(ref _pageContent, value);
        }
    }
}
