using App.Helpers;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace App.Services
{
    public class LocalFolderSettingsService : ILocalFolderSettingsService
    {
        private readonly ConcurrentDictionary<string, object> _localCache;

        public LocalFolderSettingsService()
        {
            _localCache = new ConcurrentDictionary<string, object>();
        }

        public async Task SaveSettingAsync<T>(string key, T value) where T : class
        {
            await ApplicationData.Current.LocalFolder.SaveAsync(key, JsonSerializer.Serialize(value));
            _localCache.AddOrUpdate(key, value, (k, oldValue) => value);

        }

        public async Task<T> GetSettingAsync<T>(string key) where T : class
        {
            var setting = await ApplicationData.Current.LocalFolder.ReadAsync<T>(key);
            return _localCache.GetOrAdd(key,
                val => setting) as T;
        }

    }
}
