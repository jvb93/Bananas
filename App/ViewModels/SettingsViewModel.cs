using System;
using System.Threading.Tasks;
using System.Windows.Input;

using App.Helpers;
using App.Services;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using App.Core.Services;

namespace App.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/settings.md
    public class SettingsViewModel : ObservableObject
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        public static readonly string MandrillApiKeySettingsKey = "MandrillApiKey";
        private string _mandrillApiKey;

        private readonly ILocalFolderSettingsService _settingsService;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        public string MandrillApiKey
        {
            get { return _mandrillApiKey; }

            set
            {
                SetProperty(ref _mandrillApiKey, value);
            }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        public async Task SetMandrillApiKeyAsync()
        {
            await _settingsService.SaveSettingAsync(MandrillApiKeySettingsKey, MandrillApiKey);
        }

        public SettingsViewModel(ILocalFolderSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            MandrillApiKey = await _settingsService.GetSettingAsync<string>(MandrillApiKeySettingsKey);
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
