using System;
using Windows.UI.WindowManagement;
using Mandrill.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace App.Views
{
    public sealed partial class TemplatesDetailControl : UserControl
    {
        public MandrillTemplateInfo ListMenuItem
        {
            get { return GetValue(ListMenuItemProperty) as MandrillTemplateInfo; }
            set { SetValue(ListMenuItemProperty, value); }
        }

        public static readonly DependencyProperty ListMenuItemProperty = DependencyProperty.Register("ListMenuItem", typeof(MandrillTemplateInfo), typeof(TemplatesDetailControl), new PropertyMetadata(null, OnListMenuItemPropertyChanged));

        public TemplatesDetailControl()
        {
            InitializeComponent();
        }

        private static void OnListMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TemplatesDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }

        private async void ComposeButton_Click(object sender, RoutedEventArgs e)
        {
            var composeEmailWindow = await AppWindow.TryCreateAsync();
            var composeEmailContentFrame = new Frame();
            composeEmailContentFrame.Navigate(typeof(ComposeEmailPage));
            ElementCompositionPreview.SetAppWindowContent(composeEmailWindow, composeEmailContentFrame);

            var composeEmailPage = composeEmailContentFrame.Content as ComposeEmailPage;
            composeEmailPage.ViewModel.Template = ListMenuItem;
            await composeEmailWindow.TryShowAsync();
        }
    }
}
