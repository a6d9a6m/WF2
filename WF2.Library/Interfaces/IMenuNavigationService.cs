namespace WF2.Library.Services;

public static class MenuNavigationConstant
{
    // 主菜单导航页面
    public const string MainView = "MainView";
    public const string WeatherDetailView = "WeatherDetailView";
    public const string CitiesView = "CitiesView";
    public const string SettingsView = "SettingsView";
    public const string AboutView = "AboutView";
}

public interface IMenuNavigationService
{
    void NavigateTo(string view, object? parameter = null);
}
