namespace App.Core.Services
{
    public interface IMandrillServiceFactory
    {
        IMandrillService Build(string apiKey);
    }
}