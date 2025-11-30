using WF2.Library.Services;

namespace WF2.Library.Services;

public interface ISettingsService
{
    Task<bool> GetUseModernUIAsync();
    Task SaveUseModernUIAsync(bool useModernUI);
    Task<string?> GetLastSelectedCityAsync();
    Task SaveLastSelectedCityAsync(string cityName);
    Task<bool> GetUseDarkThemeAsync();
    Task SaveUseDarkThemeAsync(bool useDarkTheme);
    Task<string> GetSelectedLanguageAsync();
    Task SaveSelectedLanguageAsync(string language);
    Task<string> GetBackgroundImagePathAsync();
    Task SaveBackgroundImagePathAsync(string imagePath);
}