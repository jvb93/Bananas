using App.ViewModels;
using Mandrill.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using FluentValidation.Results;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace App.Views
{
    public sealed partial class TemplatesDetailControl : UserControl
    {
        public MandrillTemplateInfo ListMenuItem
        {
            get { return GetValue(ListMenuItemProperty) as MandrillTemplateInfo; }
            set
            {
                SetValue(ListMenuItemProperty, value);
                ViewModel.Template = value;
            }
        }
        public ComposeEmailViewModel ViewModel { get; private set; }
        public int RowCount { get; private set; }

        public static readonly DependencyProperty ListMenuItemProperty = DependencyProperty.Register("ListMenuItem", typeof(MandrillTemplateInfo), typeof(TemplatesDetailControl), new PropertyMetadata(null, OnListMenuItemPropertyChanged));

        public TemplatesDetailControl()
        {
            InitializeComponent();
            var container = ((App)App.Current).Container;
            ViewModel = container.GetRequiredService<ComposeEmailViewModel>();
            Loaded += TemplatesDetailControl_Loaded;

        }
        private async void TemplatesDetailControl_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private static void OnListMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TemplatesDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
            control.ViewModel.Template = e.NewValue as MandrillTemplateInfo;
        }

        private async void ChooseCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var dt = await ViewModel.SelectAndParseCsv();
            FillDataGrid(dt, MergeFieldsGrid);
        }

        private void FillDataGrid(DataTable table, DataGrid grid)
        {
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = table.Columns[i].ColumnName,
                    Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                });
            }

            var collection = new ObservableCollection<object>();
            foreach (DataRow row in table.Rows)
            {
                collection.Add(row.ItemArray);
            }

            RowCount = table.Rows.Count;

            grid.ItemsSource = collection;

            var headerNames = new ObservableCollection<string>();
            foreach (DataColumn tableColumn in table.Columns)
            {
                headerNames.Add(tableColumn.ColumnName);
            }

            EmailAddressColumnSelector.ItemsSource = headerNames;

            if (headerNames.Any())
            {
                grid.Visibility = Visibility.Visible;
                EmailAddressColumnSelector.Visibility = Visibility.Visible;
                SendButton.Visibility = Visibility.Visible;
            }
            else
            {
                grid.Visibility = Visibility.Collapsed;
                EmailAddressColumnSelector.Visibility = Visibility.Collapsed;
                SendButton.Visibility = Visibility.Collapsed;
            }
             
        }


        private void EmailAddressColumnSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.RecipientAddressesDatatableColumnName = e.AddedItems[0].ToString();
        }

        private ValidationResult ValidateViewModel()
        {
            var validator = new ComposeEmailViewModel.ComposeEmailViewModelValidator();
            return validator.Validate(ViewModel);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var validationResult = ValidateViewModel();

            if (!validationResult.IsValid)
            {
                var validationErrorDialog = new ContentDialog
                {
                    Title = "Validation Error",
                    Content = validationResult.Errors.First().ErrorMessage,
                    CloseButtonText = "Ok",
                };

                await validationErrorDialog.ShowAsync();
                return;
            }

            var locationPromptDialog = new ContentDialog
            {
                Title = "Send Emails?",
                Content = $"You're about to send {RowCount} emails. Does everything look correct?",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Send"
            };

            ContentDialogResult result = await locationPromptDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.SendEmailsAsync();
            }
        }
    }
}
