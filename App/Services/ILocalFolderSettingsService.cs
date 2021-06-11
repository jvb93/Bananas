using System.Threading.Tasks;

namespace App.Services
{
    public interface ILocalFolderSettingsService
    {
        Task SaveSettingAsync<T>(string key, T value) where T : class;
        Task<T> GetSettingAsync<T>(string key) where T : class;
    }
}
