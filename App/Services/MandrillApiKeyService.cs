using App.Helpers;
using System.Threading.Tasks;
using Windows.Storage;

namespace App.Services
{
    public static class MandrillApiKeyService
    {
        private const string SettingsKey = "MandrillApiKey";

        public static string ApiKey { get; set; }

        public static async Task InitializeAsync()
        {
            ApiKey = await LoadApiKeyFromSettingsAsync();
        }

        public static async Task SetApiKeyAsync(string apiKey)
        {
            ApiKey = apiKey;
            await SaveApiKeyInSettingsAsync(ApiKey);
        }

        private static async Task<string> LoadApiKeyFromSettingsAsync()
        {
            return await ApplicationData.Current.LocalFolder.ReadAsync<string>(SettingsKey);
        }

        private static async Task SaveApiKeyInSettingsAsync(string apiKey)
        {
            await ApplicationData.Current.LocalFolder.SaveAsync(SettingsKey, apiKey);
        }
    }
}
