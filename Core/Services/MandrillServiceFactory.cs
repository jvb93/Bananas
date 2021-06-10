using Core.Services;
using Microsoft.Extensions.Logging;

namespace App.Core.Services
{
    public class MandrillServiceFactory : IMandrillServiceFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public MandrillServiceFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IMandrillService Build(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            return new MandrillService(apiKey, _loggerFactory.CreateLogger<MandrillService>());
        }
    }
}
