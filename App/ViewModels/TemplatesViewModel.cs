using Core.Services;
using Mandrill.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace App.ViewModels
{
    public class TemplatesViewModel : ObservableObject
    {
        private readonly IMandrillService _mandrillService;
        private MandrillTemplateInfo _selected;

        public MandrillTemplateInfo Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ObservableCollection<MandrillTemplateInfo> Templates { get; private set; } = new ObservableCollection<MandrillTemplateInfo>();

        public TemplatesViewModel(IMandrillService mandrillService)
        {
            _mandrillService = mandrillService;
        }

        public async Task LoadDataAsync(ListDetailsViewState viewState)
        {
            Templates.Clear();

            var data = await _mandrillService.GetTemplatesAsync();
            foreach (var item in data)
            {
                Templates.Add(item);
            }

            if (viewState == ListDetailsViewState.Both)
            {
                Selected = Templates.First();
            }
        }
    }
}
