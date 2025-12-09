using LiteDB;
using WF2.Library.Models;
using WF2.Library.Services;

namespace WF2.Services;

public class WeatherCacheService : IWeatherCacheService
{
    private const string DatabasePath = "Filename=weather.db;Connection=shared";
    private const string CollectionName = "weather_cache";

    private LiteDatabase GetDatabase()
    {
        return new LiteDatabase(DatabasePath);
    }

    public Task MigrateDatabaseAsync()
    {
        return ClearAllAsync();
    }

    public Task SaveWeatherAsync(WeatherCache weather)
    {
        return Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);

            // 检查是否已存在该城市的数据
            var existing = collection.FindOne(x => x.CityName == weather.CityName);

            if (existing != null)
            {
                // 更新现有数据，保留ID
                weather.Id = existing.Id;

                // 确保所有字段都有值，即使是默认值
                if (weather.WindDirection == null) weather.WindDirection = "--";
                if (weather.PressureMb == 0) weather.PressureMb = 1013.25; // 标准大气压
                if (weather.VisibilityKm == 0) weather.VisibilityKm = 10; // 默认能见度
                if (weather.UvIndex == 0) weather.UvIndex = 1; // 默认低紫外线指数
                if (weather.Cloud == 0) weather.Cloud = 25; // 默认少量云

                collection.Update(weather);
            }
            else
            {
                // 插入新数据，确保所有字段都有值
                if (weather.WindDirection == null) weather.WindDirection = "--";
                if (weather.PressureMb == 0) weather.PressureMb = 1013.25; // 标准大气压
                if (weather.VisibilityKm == 0) weather.VisibilityKm = 10; // 默认能见度
                if (weather.UvIndex == 0) weather.UvIndex = 1; // 默认低紫外线指数
                if (weather.Cloud == 0) weather.Cloud = 25; // 默认少量云

                collection.Insert(weather);
            }

            collection.EnsureIndex(x => x.CityName);
        });
    }

    public Task<WeatherCache?> GetWeatherAsync(string cityName)
    {
        return Task.Run<WeatherCache?>(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            return collection.FindOne(x => x.CityName == cityName);
        });
    }

    public Task<WeatherCache?> GetValidWeatherAsync(string cityName, int expirationMinutes = 10)
    {
        return Task.Run<WeatherCache?>(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            var cache = collection.FindOne(x => x.CityName == cityName);

            // 检查缓存是否存在且未过期
            if (cache != null && !cache.IsExpired(expirationMinutes))
            {
                return cache;
            }

            return null;
        });
    }

    public Task<List<string>> GetAllCitiesAsync()
    {
        return Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            return collection.FindAll().Select(x => x.CityName).OrderByDescending(x => x).ToList();
        });
    }

    public Task<List<WeatherCache>> GetFavoriteCitiesAsync()
    {
        return Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            var result = collection.Find(x => x.IsFavorite).ToList();
            // 确保返回空列表而不是null
            return result ?? new List<WeatherCache>();
        });
    }

    public Task UpdateFavoriteStatusAsync(string cityName, bool isFavorite)
    {
        return Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            var weather = collection.FindOne(x => x.CityName == cityName);
            if (weather != null)
            {
                weather.IsFavorite = isFavorite;
                collection.Update(weather);
            }
        });
    }

    public Task DeleteWeatherAsync(string cityName)
    {
        return Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            collection.DeleteMany(x => x.CityName == cityName);
        });
    }

    public Task ClearAllAsync()
    {
        return Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            collection.DeleteAll();
        });
    }
}
