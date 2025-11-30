using Moq;
using WF2.Library.Interfaces;
using WF2.Library.Services;
using WF2.Library.Models;
using WF2.Library.ViewModels;

namespace WF2UTest.ViewModels;

[TestFixture]
public class MainViewModelTests
{
    private Mock<IWeatherCacheService> _mockCacheService = null!;
    private Mock<ISettingsService> _mockSettingsService = null!;
    private Mock<ILocalizationService> _mockLocalizationService = null!;
    private Mock<IBackgroundImageService> _mockBackgroundImageService = null!;
    private MainViewModel _viewModel = null!;

    [SetUp]
    public void Setup()
    {
        _mockCacheService = new Mock<IWeatherCacheService>();
        _mockSettingsService = new Mock<ISettingsService>();
        _mockLocalizationService = new Mock<ILocalizationService>();
        _mockBackgroundImageService = new Mock<IBackgroundImageService>();

        // Setup default localization strings
        _mockLocalizationService.Setup(x => x.GetString(It.IsAny<string>())).Returns((string key) => key);

        // Setup default settings
        _mockSettingsService.Setup(x => x.GetUseDarkThemeAsync()).ReturnsAsync(true);
        _mockSettingsService.Setup(x => x.GetSelectedLanguageAsync()).ReturnsAsync("中文");
        _mockSettingsService.Setup(x => x.GetLastSelectedCityAsync()).ReturnsAsync("Beijing");
        _mockBackgroundImageService.Setup(x => x.GetBackgroundImagePathAsync()).ReturnsAsync(string.Empty);

        _viewModel = new MainViewModel(
            _mockCacheService.Object,
            _mockSettingsService.Object,
            _mockLocalizationService.Object,
            _mockBackgroundImageService.Object
        );
    }

    [Test]
    public async Task InitializeAsync_ShouldLoadSettings()
    {
        // Arrange - already set up in Setup()

        // Act
        await Task.Delay(500); // 等待初始化完成

        // Assert
        _mockSettingsService.Verify(x => x.GetSelectedLanguageAsync(), Times.AtLeastOnce);
        _mockSettingsService.Verify(x => x.GetUseDarkThemeAsync(), Times.AtLeastOnce);
    }

    [Test]
    public async Task InitializeAsync_ShouldLoadBackgroundImage()
    {
        // Arrange - already set up in Setup()

        // Act
        await Task.Delay(500); // 等待初始化完成

        // Assert
        _mockBackgroundImageService.Verify(x => x.GetBackgroundImagePathAsync(), Times.AtLeastOnce);
    }

    [Test]
    public void LocationName_ShouldBeInitializedCorrectly()
    {
        // Assert
        Assert.That(_viewModel.LocationName, Is.Not.Null);
        Assert.That(_viewModel.LocationName, Is.Not.Empty);
    }

    [Test]
    public void Temperature_ShouldBeInitializedCorrectly()
    {
        // Assert
        Assert.That(_viewModel.Temperature, Is.Not.Null);
        Assert.That(_viewModel.Temperature, Does.Contain("°C"));
    }

    [Test]
    public void ConditionText_ShouldBeInitializedCorrectly()
    {
        // Assert
        Assert.That(_viewModel.ConditionText, Is.Not.Null);
    }

    [Test]
    public void WeatherIcon_ShouldBeInitializedCorrectly()
    {
        // Assert
        Assert.That(_viewModel.WeatherIcon, Is.Not.Null);
        Assert.That(_viewModel.WeatherIcon, Is.Not.Empty);
    }

    [Test]
    public void UseDarkTheme_ShouldBeSetFromSettings()
    {
        // Assert
        Assert.That(_viewModel.UseDarkTheme, Is.True);
    }
}
