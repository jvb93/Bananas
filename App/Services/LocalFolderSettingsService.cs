using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using App.Helpers;

namespace App.Services
{
    public class LocalFolderSettingsService : ILocalFolderSettingsService
    {
        private ConcurrentDictionary<string, object> _localCache;

        public async Task SaveSettingAsync<T>(string key, T value) where T : class
        {
            await ApplicationData.Current.LocalFolder.SaveAsync(key, JsonSerializer.Serialize(value));
            _localCache.AddOrUpdate(key, value, (k, oldValue) => value);

        }

        public async Task<T> GetSettingAsync<T>(string key) where T : class
        {
            return _localCache.GetOrAdd(key,
                val => new Lazy<T>(() => ApplicationData.Current.LocalFolder.ReadAsync<T>(key)
                    .GetAwaiter()
                    .GetResult())) as T;
        }

    }
}
