using System;
using App.Core.Services;
using Core.Classes;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

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

            serviceCollection.Configure<MandrillSettings>(opt => configuration.GetSection("Mandrill").Bind(opt));
            
            return serviceCollection;
        }
        public static IServiceCollection AddStructuredLogging(this IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.File(new JsonFormatter(), "logs/logs.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 10000000, rollOnFileSizeLimit: true)
                .CreateLogger();
            
            serviceCollection
                .AddLogging(builder => builder.AddSerilog());
            
            return serviceCollection;
        }
        public static IServiceCollection AddMandrillServiceFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMandrillServiceFactory, MandrillServiceFactory>();
            return serviceCollection;
        }
    }
}