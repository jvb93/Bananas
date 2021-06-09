using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Services;
using CsvHelper;
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
            System.Console.WriteLine("Choose a template:");
            foreach (var template in templates)
            {
                System.Console.WriteLine(template.Name);
            }

            var selectedTemplate = System.Console.ReadLine();
           
            System.Console.WriteLine("CSV path:");
            var csvPath = System.Console.ReadLine();

            System.Console.WriteLine("Email Subject:");
            var subject = System.Console.ReadLine();

            System.Console.WriteLine("Email From Address:");
            var fromAddress = System.Console.ReadLine();

            System.Console.WriteLine("Email From Name:");
            var fromName = System.Console.ReadLine();


            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var anonymousTypeDefinition = new
                {
                    Address = string.Empty,
                    Tracking = string.Empty
                };

                var records = csv.GetRecords(anonymousTypeDefinition).ToList();

                System.Console.WriteLine("========================");
                System.Console.WriteLine($"YOU ARE ABOUT TO SEND {records.Count} EMAILS. CONFIRM THE FOLLOWING:");
                System.Console.WriteLine($"Template: {selectedTemplate}");
                System.Console.WriteLine($"Subject: {subject}");
                System.Console.WriteLine($"From Address: {fromAddress}");
                System.Console.WriteLine($"From Name: {fromName}");

                System.Console.WriteLine("CONTINUE? (Y/N):");
                var selection = System.Console.ReadLine().ToUpper();

                if (selection != "Y")
                {
                    System.Console.WriteLine("ABORTING!");
                    return;
                }

                for (var x = 0; x < records.Count(); x++)
                {
                    System.Console.WriteLine($"Sending {x+1} of {records.Count()}");
                    var templateFields = new Dictionary<string, string>()
                    {
                        {"tracking", records[x].Tracking}
                    };
                    await mandrillService.SendMessageAsync(records[x].Address, subject, fromAddress, fromName,
                        selectedTemplate, templateFields);
                    Thread.Sleep(250);
                }
            }
            System.Console.WriteLine("Done. Press any key to continue...");
            System.Console.ReadKey();
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