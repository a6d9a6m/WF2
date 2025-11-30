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
                // 更新现有数据
                weather.Id = existing.Id;
                collection.Update(weather);
            }
            else
            {
                // 插入新数据
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
