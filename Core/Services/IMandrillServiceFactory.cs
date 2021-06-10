using Core.Services;

namespace App.Core.Services
{
    public interface IMandrillServiceFactory
    {
        IMandrillService Build(string apiKey);
    }
}