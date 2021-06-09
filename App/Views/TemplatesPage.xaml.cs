using App.ViewModels;
using Core.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace App.Views
{
    public sealed partial class TemplatesPage : Page
    {
        public TemplatesViewModel ViewModel { get; private set; } 

        public TemplatesPage()
        {
            InitializeComponent();
            var container = ((App)App.Current).Container;
            ViewModel = ActivatorUtilities.GetServiceOrCreateInstance(container, typeof(TemplatesViewModel)) as TemplatesViewModel;
            Loaded += ListDetailPage_Loaded;
        }

        private async void ListDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
        }
    }
}
