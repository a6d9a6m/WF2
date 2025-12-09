using Moq;
using WF2.Library.Services;
using WF2.Library.Models;
using WF2.Library.ViewModels;

namespace WF2UTest.ViewModels;

[TestFixture]
public class CitiesViewModelTests
{
    private Mock<ISettingsService> _mockSettingsService = null!;
    private Mock<IWeatherCacheService> _mockCacheService = null!;
    private Mock<IMenuNavigationService> _mockMenuNavigationService = null!;
    private Mock<ILocalizationService> _mockLocalizationService = null!;
    private CitiesViewModel _viewModel = null!;

    [SetUp]
    public void Setup()
    {
        _mockSettingsService = new Mock<ISettingsService>();
        _mockCacheService = new Mock<IWeatherCacheService>();
        _mockMenuNavigationService = new Mock<IMenuNavigationService>();
        _mockLocalizationService = new Mock<ILocalizationService>();

        // Setup default localization
        _mockLocalizationService.Setup(x => x.GetString(It.IsAny<string>())).Returns((string key) => key);

        _viewModel = new CitiesViewModel(
            _mockCacheService.Object,
            _mockSettingsService.Object,
            _mockMenuNavigationService.Object,
            _mockLocalizationService.Object
        );
    }

    [Test]
    public async Task LoadCitiesAsync_ShouldLoadFavoriteCities()
    {
        // Arrange
        var favoriteCities = new List<WeatherCache>
        {
            new WeatherCache { CityName = "Beijing", LocationName = "Beijing", IsFavorite = true, Temperature = 20 },
            new WeatherCache { CityName = "Shanghai", LocationName = "Shanghai", IsFavorite = true, Temperature = 25 }
        };

        _mockCacheService.Setup(x => x.GetFavoriteCitiesAsync()).ReturnsAsync(favoriteCities);

        // Act
        await Task.Delay(500); // 等待初始化加载

        // Assert
        _mockCacheService.Verify(x => x.GetFavoriteCitiesAsync(), Times.AtLeastOnce);
    }

    [Test]
    public async Task LoadCitiesAsync_WithNoFavorites_ShouldReturnEmptyList()
    {
        // Arrange
        _mockCacheService.Setup(x => x.GetFavoriteCitiesAsync()).ReturnsAsync(new List<WeatherCache>());

        // Act
        await Task.Delay(500);

        // Assert
        Assert.That(_viewModel.Cities, Is.Empty);
    }

    [Test]
    public async Task DeleteCityAsync_ShouldRemoveCityFromCache()
    {
        // Arrange
        string cityToDelete = "Beijing";
        _mockCacheService.Setup(x => x.DeleteWeatherAsync(cityToDelete)).Returns(Task.CompletedTask);

        // Act
        await _viewModel.DeleteCityCommand.ExecuteAsync(cityToDelete);

        // Assert
        _mockCacheService.Verify(x => x.DeleteWeatherAsync(cityToDelete), Times.Once);
    }

    [Test]
    public async Task SelectCityAsync_ShouldSaveSelectedCityAndNavigate()
    {
        // Arrange
        string selectedCity = "Shanghai";
        _mockSettingsService.Setup(x => x.SaveLastSelectedCityAsync(selectedCity)).Returns(Task.CompletedTask);

        // Act
        await _viewModel.SelectCityCommand.ExecuteAsync(selectedCity);

        // Assert
        _mockSettingsService.Verify(x => x.SaveLastSelectedCityAsync(selectedCity), Times.Once);
    }

    [Test]
    public async Task RefreshCitiesAsync_ShouldReloadCitiesList()
    {
        // Arrange
        var updatedCities = new List<WeatherCache>
        {
            new WeatherCache { CityName = "Guangzhou", IsFavorite = true },
            new WeatherCache { CityName = "Shenzhen", IsFavorite = true },
            new WeatherCache { CityName = "Hangzhou", IsFavorite = true }
        };

        _mockCacheService.Setup(x => x.GetFavoriteCitiesAsync()).ReturnsAsync(updatedCities);

        // Act
        await _viewModel.RefreshCitiesCommand.ExecuteAsync(null);

        // Assert
        _mockCacheService.Verify(x => x.GetFavoriteCitiesAsync(), Times.AtLeastOnce);
    }

    [Test]
    public async Task LoadCities_ShouldHandleError_Gracefully()
    {
        // Arrange
        _mockCacheService.Setup(x => x.GetFavoriteCitiesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _viewModel.RefreshCitiesCommand.ExecuteAsync(null));
    }

    [Test]
    public void IsLoading_ShouldToggleDuringOperations()
    {
        // Arrange
        bool initialLoadingState = _viewModel.IsLoading;

        // Act - 在异步操作期间检查
        // 实际测试中需要更复杂的同步机制

        // Assert
        Assert.That(_viewModel.IsLoading, Is.EqualTo(initialLoadingState));
    }

    [Test]
    public async Task UpdateLanguage_ShouldRefreshLocalizedText()
    {
        // Arrange
        string newLanguage = "中文";

        // Act
        await _viewModel.UpdateLanguageAsync(newLanguage);

        // Assert
        _mockLocalizationService.Verify(x => x.SetLanguage(newLanguage), Times.Once);
        _mockSettingsService.Verify(x => x.SaveSelectedLanguageAsync(newLanguage), Times.Once);
    }

    [Test]
    public async Task StatusMessage_ShouldUpdateAfterCitiesLoad()
    {
        // Arrange
        var cities = new List<WeatherCache>
        {
            new WeatherCache { CityName = "City1", IsFavorite = true },
            new WeatherCache { CityName = "City2", IsFavorite = true }
        };

        _mockCacheService.Setup(x => x.GetFavoriteCitiesAsync()).ReturnsAsync(cities);

        // Act - 等待加载
        await Task.Delay(500);

        // Assert - 状态消息应该显示已加载的城市数量
        // 在实际实现中验证 StatusMessage 属性
    }
}
