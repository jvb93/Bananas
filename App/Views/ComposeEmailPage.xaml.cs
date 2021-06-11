using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using App.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using App.Core.Models;
using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComposeEmailPage : Page
    {
        public ComposeEmailViewModel ViewModel { get; private set; }

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

        public static void FillDataGrid(DataTable table, DataGrid grid)
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

            grid.ItemsSource = collection;
        }


    }
}
