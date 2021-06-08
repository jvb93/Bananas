using System;
using System.Threading.Tasks;
using Core;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Console
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        static async Task Main(string[] args)
        {
            BuildServiceProvider();
            
            var scope = _serviceProvider.CreateScope();
            var mandrillService = scope.ServiceProvider.GetRequiredService<IMandrillService>();

            var templates = await mandrillService.GetTemplatesAsync();
            System.Console.WriteLine("Templates:");
            foreach (var template in templates)
            {
                System.Console.WriteLine(template.Name);
            }
            
            DisposeServiceProvider();
        }

        static void BuildServiceProvider()
        {
            var services = new ServiceCollection();
            
            services
                .AddConfiguration()
                .AddStructuredLogging()
                .AddMandrill();
            
            _serviceProvider = services.BuildServiceProvider(true);
        }
        
        private static void DisposeServiceProvider()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}