using WF2.Library.Services;
using WF2.Library.Models;
using WF2.Services;

namespace WF2UTest.Services;

[TestFixture]
public class WeatherCacheServiceTests
{
    private WeatherCacheService _cacheService = null!;
    private string _testDbPath = null!;

    [SetUp]
    public void Setup()
    {
        // 使用临时数据库文件进行测试
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_cache_{Guid.NewGuid()}.db");
        _cacheService = new WeatherCacheService($"Filename={_testDbPath};Connection=shared");
    }

    [TearDown]
    public void TearDown()
    {
        // 清理测试数据库文件
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    private WeatherCache CreateTestWeatherCache(string cityName, bool isFavorite = false)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return new WeatherCache
        {
            CityName = cityName,
            LocationName = cityName,
            Country = "China",
            Temperature = 25.0,
            ConditionText = "Sunny",
            ConditionIcon = "sunny.png",
            Humidity = 60,
            WindKph = 10.5,
            FeelsLikeC = 26.0,
            LastUpdated = now,
            CachedAt = now,
            IsFavorite = isFavorite
        };
    }

    [Test]
    public async Task SaveWeatherAsync_ShouldSaveWeatherData()
    {
        // Arrange
        var weather = CreateTestWeatherCache("Beijing");

        // Act
        await _cacheService.SaveWeatherAsync(weather);
        var result = await _cacheService.GetWeatherAsync("Beijing");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CityName, Is.EqualTo("Beijing"));
        Assert.That(result.Temperature, Is.EqualTo(25.0));
        Assert.That(result.ConditionText, Is.EqualTo("Sunny"));
    }

    [Test]
    public async Task GetWeatherAsync_WithNonExistentCity_ShouldReturnNull()
    {
        // Act
        var result = await _cacheService.GetWeatherAsync("NonExistentCity");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SaveWeatherAsync_ShouldOverwriteExistingData()
    {
        // Arrange
        var weather1 = CreateTestWeatherCache("Shanghai");
        weather1.Temperature = 20.0;

        var weather2 = CreateTestWeatherCache("Shanghai");
        weather2.Temperature = 30.0;

        // Act
        await _cacheService.SaveWeatherAsync(weather1);
        await _cacheService.SaveWeatherAsync(weather2);
        var result = await _cacheService.GetWeatherAsync("Shanghai");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Temperature, Is.EqualTo(30.0));
    }

    [Test]
    public async Task GetAllCitiesAsync_ShouldReturnAllCityNames()
    {
        // Arrange
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Beijing"));
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Shanghai"));
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Guangzhou"));

        // Act
        var cities = await _cacheService.GetAllCitiesAsync();

        // Assert
        Assert.That(cities, Has.Count.EqualTo(3));
        Assert.That(cities, Does.Contain("Beijing"));
        Assert.That(cities, Does.Contain("Shanghai"));
        Assert.That(cities, Does.Contain("Guangzhou"));
    }

    [Test]
    public async Task GetFavoriteCitiesAsync_ShouldReturnOnlyFavoriteCities()
    {
        // Arrange
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Beijing", isFavorite: true));
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Shanghai", isFavorite: false));
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Guangzhou", isFavorite: true));

        // Act
        var favorites = await _cacheService.GetFavoriteCitiesAsync();

        // Assert
        Assert.That(favorites, Has.Count.EqualTo(2));
        Assert.That(favorites.Select(c => c.CityName), Does.Contain("Beijing"));
        Assert.That(favorites.Select(c => c.CityName), Does.Contain("Guangzhou"));
        Assert.That(favorites.Select(c => c.CityName), Does.Not.Contain("Shanghai"));
    }

    [Test]
    public async Task UpdateFavoriteStatusAsync_ShouldUpdateFavoriteStatus()
    {
        // Arrange
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Beijing", isFavorite: false));

        // Act
        await _cacheService.UpdateFavoriteStatusAsync("Beijing", true);
        var result = await _cacheService.GetWeatherAsync("Beijing");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.IsFavorite, Is.True);
    }

    [Test]
    public async Task UpdateFavoriteStatusAsync_WithNonExistentCity_ShouldNotThrowException()
    {
        // Act & Assert
        Assert.DoesNotThrowAsync(async () =>
            await _cacheService.UpdateFavoriteStatusAsync("NonExistent", true));
    }

    [Test]
    public async Task DeleteWeatherAsync_ShouldRemoveWeatherData()
    {
        // Arrange
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Beijing"));

        // Act
        await _cacheService.DeleteWeatherAsync("Beijing");
        var result = await _cacheService.GetWeatherAsync("Beijing");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ClearAllAsync_ShouldRemoveAllWeatherData()
    {
        // Arrange
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Beijing"));
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Shanghai"));
        await _cacheService.SaveWeatherAsync(CreateTestWeatherCache("Guangzhou"));

        // Act
        await _cacheService.ClearAllAsync();
        var cities = await _cacheService.GetAllCitiesAsync();

        // Assert
        Assert.That(cities, Is.Empty);
    }

    [Test]
    public void WeatherCache_FromApiResponse_ShouldCreateValidCache()
    {
        // Arrange
        var apiResponse = new WeatherApiResponse
        {
            Location = new Location
            {
                Name = "TestCity",
                Country = "TestCountry",
                Region = "TestRegion",
                Lat = 0,
                Lon = 0,
                Localtime = "2024-01-01 12:00"
            },
            Current = new Current
            {
                TempC = 20.5,
                TempF = 68.9,
                Humidity = 70,
                WindKph = 15.0,
                WindMph = 9.3,
                WindDir = "NE",
                PressureMb = 1013.0,
                PressureIn = 29.91,
                VisKm = 10.0,
                VisMiles = 6.2,
                Uv = 5.0,
                Cloud = 25,
                Condition = new Condition
                {
                    Text = "Partly cloudy",
                    Icon = "cloudy.png",
                    Code = 1003
                }
            }
        };

        // Act
        var cache = WeatherCache.FromApiResponse(apiResponse);

        // Assert
        Assert.That(cache.CityName, Is.EqualTo("TestCity"));
        Assert.That(cache.Country, Is.EqualTo("TestCountry"));
        Assert.That(cache.Temperature, Is.EqualTo(20.5));
        Assert.That(cache.ConditionText, Is.EqualTo("Partly cloudy"));
        Assert.That(cache.Humidity, Is.EqualTo(70));
        Assert.That(cache.WindKph, Is.EqualTo(15.0));
        Assert.That(cache.IsFavorite, Is.False);
    }
}
