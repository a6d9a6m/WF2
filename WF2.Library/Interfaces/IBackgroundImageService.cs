using System.Threading.Tasks;

namespace WF2.Library.Interfaces
{
    public interface IBackgroundImageService
    {
        Task<string> GetBackgroundImagePathAsync();
        Task<string> GetBackgroundForWeatherConditionAsync(string condition);
        Task<bool> SetCustomBackgroundAsync(string imagePath);
        Task CleanExpiredCacheAsync(int maxAgeDays = 7);
        Task LimitCacheSizeAsync(int maxFiles = 5);
    }
}