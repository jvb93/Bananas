using App.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Core.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

            serviceCollection.Configure<MandrillSettings>(opt => configuration.GetSection("Mandrill").Bind(opt));
            
            return serviceCollection;
        }
       
        public static IServiceCollection AddMandrillServiceFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMandrillServiceFactory, MandrillServiceFactory>();
            return serviceCollection;
        }
    }
}