using App.Core.Services;
using App.Services;
using Core.Services;
using Mandrill.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using CsvHelper;

namespace App.ViewModels
{
    public class ComposeEmailViewModel : ObservableObject
    {
        private IMandrillService _mandrillService;
        private readonly IMandrillServiceFactory _mandrillServiceFactory;
        private readonly ILocalFolderSettingsService _settingsService;


        public MandrillTemplateInfo Template { get;  set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string RecipientAddressesDatatableColumnName { get; set; }
        private DataTable MergeTags { get; set; }
        public ComposeEmailViewModel(IMandrillServiceFactory mandrillServiceFactory, ILocalFolderSettingsService settingsService)
        {
            _mandrillServiceFactory = mandrillServiceFactory;
            _settingsService = settingsService;
        }

        public async Task LoadDataAsync()
        {

        }

        public async Task<DataTable> SelectAndParseCsv()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".csv");

            var file = await picker.PickSingleFileAsync();
            return await ExtractRecipientDataFromCsvAsync(file);

        }

        private async Task<DataTable> ExtractRecipientDataFromCsvAsync(StorageFile csvFile)
        {
            using (var reader = new StreamReader((await csvFile.OpenReadAsync()).AsStreamForRead()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                using (var dr = new CsvDataReader(csv))
                {
                    MergeTags = new DataTable();
                    MergeTags.Load(dr);
                    return MergeTags;
                }
            }
        }

        public async Task InitializeAsync()
        {
            var mandrillApiKey = await _settingsService.GetSettingAsync<string>(SettingsViewModel.MandrillApiKeySettingsKey);
            _mandrillService = _mandrillServiceFactory.Build(mandrillApiKey);
            
        }

        private Dictionary<string, string> ConvertDynamicToDictionary(dynamic rawObject, params object[] excludableColumns)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(rawObject))
            {
                if (excludableColumns.Contains(propertyDescriptor.Name))
                {
                    continue;
                }
                var obj = propertyDescriptor.GetValue(rawObject).ToString();
                dictionary.Add(propertyDescriptor.Name, obj);
            }
            return dictionary;

        }

        public async Task SendEmailsAsync()
        {
            if (_mandrillService != null)
            {
                for (int row = 0; row < MergeTags.Rows.Count; row++)
                {
                    var rowMergeTagDictionary = new Dictionary<string, string>();
                    for (int col = 0; col < MergeTags.Columns.Count; col++)
                    {
                        rowMergeTagDictionary.Add(MergeTags.Columns[col].ColumnName, MergeTags.Rows[row].ItemArray[col].ToString());

                    }
                    await _mandrillService.SendMessageAsync(rowMergeTagDictionary[RecipientAddressesDatatableColumnName], Subject, FromAddress, FromName, Template.Name, rowMergeTagDictionary);
                    await Task.Delay(250);
                }
            }
        }
    }
}
