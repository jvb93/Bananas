using Mandrill.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using App.Core.Services;
using App.Services;

namespace App.ViewModels
{
    public class TemplatesViewModel : ObservableObject
    {
        private IMandrillService _mandrillService;
        private readonly IMandrillServiceFactory _mandrillServiceFactory;
        private readonly ILocalFolderSettingsService _settingsService;
        private MandrillTemplateInfo _selected;

        public MandrillTemplateInfo Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ObservableCollection<MandrillTemplateInfo> Templates { get; private set; } = new ObservableCollection<MandrillTemplateInfo>();

        public TemplatesViewModel(IMandrillServiceFactory mandrillServiceFactory, ILocalFolderSettingsService settingsService)
        {
            _mandrillServiceFactory = mandrillServiceFactory;
            _settingsService = settingsService;
        }

        public async Task LoadDataAsync(ListDetailsViewState viewState)
        {
            Templates.Clear();
            if(_mandrillService != null)
            {
                var data = await _mandrillService.GetTemplatesAsync();
                foreach (var item in data)
                {
                    Templates.Add(item);
                }

                if (viewState == ListDetailsViewState.Both && Templates?.Any() == true)
                {
                    Selected = Templates.First();
                }
            }
           
        }

        public async Task InitializeAsync()
        {
            var mandrillApiKey = await _settingsService.GetSettingAsync<string>(SettingsViewModel.MandrillApiKeySettingsKey);
            _mandrillService = _mandrillServiceFactory.Build(mandrillApiKey);
            
        }
    }
}
