using Core.Services;

namespace App.Core.Services
{
    public class MandrillServiceFactory : IMandrillServiceFactory
    {

        public MandrillServiceFactory()
        {
        }

        public IMandrillService Build(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            return new MandrillService(apiKey);
        }
    }
}
