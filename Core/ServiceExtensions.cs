using System;
using Core.Classes;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Core
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

            serviceCollection.Configure<AppSettings>(opt => configuration.GetSection("Mandrill").Bind(opt));
            
            return serviceCollection;
        }
        public static IServiceCollection AddStructuredLogging(this IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File($"logs/log-{DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss")}")
                .CreateLogger();
            
            serviceCollection
                .AddLogging(builder => builder.AddSerilog());
            
            return serviceCollection;
        }
        public static IServiceCollection AddMandrill(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMandrillService, MandrillService>();
            return serviceCollection;
        }
    }
}