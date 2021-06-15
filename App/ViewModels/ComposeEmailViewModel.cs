using App.Core.Models;
using App.Core.Services;
using App.Services;
using CsvHelper;
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

namespace App.ViewModels
{
    public class ComposeEmailViewModel : ObservableObject
    {
        private IMandrillService _mandrillService;
        private readonly IMandrillServiceFactory _mandrillServiceFactory;
        private readonly ILocalFolderSettingsService _settingsService;
        private readonly IMandrillTaskService _mandrillTaskService;

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
            _mandrillTaskService = new MandrillTaskService();
        }

        public async Task<DataTable> SelectAndParseCsv()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".csv");

            var file = await picker.PickSingleFileAsync();
            if (file != null && file.IsAvailable)
            {
                return await ExtractRecipientDataFromCsvAsync(file);

            }

            return new DataTable();
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

        public async Task SendEmailsAsync()
        {
            if (_mandrillService != null)
            {
                var taskBatch = new TaskBatch();
                for (int row = 0; row < MergeTags.Rows.Count; row++)
                {
                    var rowMergeTagDictionary = new Dictionary<string, string>();
                    for (int col = 0; col < MergeTags.Columns.Count; col++)
                    {
                        rowMergeTagDictionary.Add(MergeTags.Columns[col].ColumnName, MergeTags.Rows[row].ItemArray[col].ToString());

                    }
                    taskBatch.Tasks.Add(new MandrillSendTemplateTask()
                    {
                        Recipient = rowMergeTagDictionary[RecipientAddressesDatatableColumnName],
                        Subject = Subject,
                        FromName = FromName,
                        FromAddress = FromAddress,
                        TemplateName = Template.Name,
                        MergeVariables = rowMergeTagDictionary
                    });

                    await _mandrillTaskService.EnqueueTaskBatch(taskBatch, _mandrillService);
                }
            }
        }
    }
}
