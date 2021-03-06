using System;

using App.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using App.Core.Services;
using App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace App
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        public IServiceProvider Container { get; }

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            Container = ConfigureDependencyInjection();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.TemplatesPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        private IServiceProvider ConfigureDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddMandrillServiceFactory();

            serviceCollection.AddTransient<SettingsViewModel>();
            serviceCollection.AddTransient<TemplatesViewModel>();
            serviceCollection.AddTransient<ComposeEmailViewModel>();
            serviceCollection.AddSingleton<ILocalFolderSettingsService, LocalFolderSettingsService>();
            serviceCollection.AddSingleton<IMandrillTaskService, MandrillTaskService>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
