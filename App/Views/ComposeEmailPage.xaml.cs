using System;
using App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComposeEmailPage : Page
    {
        public ComposeEmailViewModel ViewModel { get; private set; }
        public int RowCount { get; private set; }

        public ComposeEmailPage()
        {
            InitializeComponent();
            var container = ((App)App.Current).Container;
            ViewModel = container.GetRequiredService<ComposeEmailViewModel>();
            Loaded += ComposeEmailPage_Loaded;
        }

        private async void ComposeEmailPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
            await ViewModel.LoadDataAsync();
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

            grid.Visibility = Visibility.Visible;
            EmailAddressColumnSelector.Visibility = Visibility.Visible;
        }


        private void EmailAddressColumnSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.RecipientAddressesDatatableColumnName = e.AddedItems[0].ToString();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog locationPromptDialog = new ContentDialog
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
