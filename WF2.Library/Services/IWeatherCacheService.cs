using WF2.Library.Models;

namespace WF2.Library.Services;

public interface IWeatherCacheService
{
    Task SaveWeatherAsync(WeatherCache weather);
    Task<WeatherCache?> GetWeatherAsync(string cityName);
    Task<List<string>> GetAllCitiesAsync();
    Task<List<WeatherCache>> GetFavoriteCitiesAsync();
    Task UpdateFavoriteStatusAsync(string cityName, bool isFavorite);
    Task DeleteWeatherAsync(string cityName);
    Task ClearAllAsync();
}
