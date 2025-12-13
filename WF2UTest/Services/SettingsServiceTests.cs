using WF2.Library.Services;

namespace WF2UTest.Services;

[TestFixture]
public class SettingsServiceTests
{
    private SettingsService _settingsService = null!;
    private string _testDbPath = null!;

    [SetUp]
    public void Setup()
    {
        // 使用临时数据库文件进行测试
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_settings_{Guid.NewGuid()}.db");
        _settingsService = new SettingsService();
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

    [Test]
    public async Task SaveAndGetSelectedLanguageAsync_ShouldSaveAndRetrieveLanguage()
    {
        // Arrange
        string expectedLanguage = "English";

        // Act
        await _settingsService.SaveSelectedLanguageAsync(expectedLanguage);
        string actualLanguage = await _settingsService.GetSelectedLanguageAsync();

        // Assert
        Assert.That(actualLanguage, Is.EqualTo(expectedLanguage));
    }

    [Test]
    public async Task GetSelectedLanguageAsync_WithNoSavedLanguage_ShouldReturnDefaultLanguage()
    {
        // Act
        string language = await _settingsService.GetSelectedLanguageAsync();

        // Assert
        Assert.That(language, Is.Not.Null);
        Assert.That(language, Is.Not.Empty);
    }

    [Test]
    public async Task SaveAndGetUseDarkThemeAsync_ShouldSaveAndRetrieveTheme()
    {
        // Arrange
        bool expectedDarkTheme = true;

        // Act
        await _settingsService.SaveUseDarkThemeAsync(expectedDarkTheme);
        bool actualDarkTheme = await _settingsService.GetUseDarkThemeAsync();

        // Assert
        Assert.That(actualDarkTheme, Is.EqualTo(expectedDarkTheme));
    }

    [Test]
    public async Task SaveAndGetLastSelectedCityAsync_ShouldSaveAndRetrieveCity()
    {
        // Arrange
        string expectedCity = "Shanghai";

        // Act
        await _settingsService.SaveLastSelectedCityAsync(expectedCity);
        string? actualCity = await _settingsService.GetLastSelectedCityAsync();

        // Assert
        Assert.That(actualCity, Is.EqualTo(expectedCity));
    }

    [Test]
    public async Task SaveAndGetBackgroundImagePathAsync_ShouldSaveAndRetrievePath()
    {
        // Arrange
        string expectedPath = "/path/to/background.jpg";

        // Act
        await _settingsService.SaveBackgroundImagePathAsync(expectedPath);
        string? actualPath = await _settingsService.GetBackgroundImagePathAsync();

        // Assert
        Assert.That(actualPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public async Task MultipleOperations_ShouldMaintainIndependentSettings()
    {
        // Arrange
        string language = "中文";
        bool darkTheme = false;
        string city = "Guangzhou";

        // Act
        await _settingsService.SaveSelectedLanguageAsync(language);
        await _settingsService.SaveUseDarkThemeAsync(darkTheme);
        await _settingsService.SaveLastSelectedCityAsync(city);

        // Assert
        Assert.That(await _settingsService.GetSelectedLanguageAsync(), Is.EqualTo(language));
        Assert.That(await _settingsService.GetUseDarkThemeAsync(), Is.EqualTo(darkTheme));
        Assert.That(await _settingsService.GetLastSelectedCityAsync(), Is.EqualTo(city));
    }

    [Test]
    public async Task OverwriteSettings_ShouldUpdateExistingValues()
    {
        // Arrange
        await _settingsService.SaveSelectedLanguageAsync("English");
        string newLanguage = "中文";

        // Act
        await _settingsService.SaveSelectedLanguageAsync(newLanguage);
        string result = await _settingsService.GetSelectedLanguageAsync();

        // Assert
        Assert.That(result, Is.EqualTo(newLanguage));
    }
}
