using App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace App.Views
{
    public sealed partial class TemplatesPage : Page
    {
        public TemplatesViewModel ViewModel { get; private set; } 

        public TemplatesPage()
        {
            InitializeComponent();
            var container = ((App)App.Current).Container;
            ViewModel = container.GetRequiredService<TemplatesViewModel>();
            Loaded += ListDetailPage_Loaded;
        }

        private async void ListDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
            await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
        }
    }
}
