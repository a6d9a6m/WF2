using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace WF2.Library.Services
{
    public interface ILocalizationService
    {
        string GetString(string key);
        void SetLanguage(string language);
        string GetCurrentLanguage();
        event EventHandler? LanguageChanged;
    }

    public class LocalizationService : ILocalizationService
    {
        private Dictionary<string, string> _translations = new();
        private string _currentLanguage = "中文";
        public event EventHandler? LanguageChanged;

        public LocalizationService()
        {
            LoadLanguage(_currentLanguage);
        }

        public string GetString(string key)
        {
            if (_translations.TryGetValue(key, out var value))
            {
                return value;
            }
            return key; // 如果找不到翻译，返回键名
        }

        public void SetLanguage(string language)
        {
            if (_currentLanguage != language)
            {
                _currentLanguage = language;
                LoadLanguage(language);
                LanguageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string GetCurrentLanguage()
        {
            return _currentLanguage;
        }

        private void LoadLanguage(string language)
        {
            _translations.Clear();
            
            // 根据语言加载对应的翻译
            if (language == "English")
            {
                LoadEnglishTranslations();
            }
            else
            {
                LoadChineseTranslations();
            }
        }

        private void LoadChineseTranslations()
        {
            // 主界面
            _translations["AppTitle"] = "天气预报助手";
            _translations["WeatherAssistant"] = "天气预报助手";
            _translations["NavigationMenu"] = "导航菜单";
            _translations["WeatherHome"] = "天气首页";
            _translations["WeatherForecast"] = "天气预报";
            _translations["RealTimeWeatherQuerySystem"] = "实时天气查询系统";
            _translations["Greeting"] = "欢迎使用天气助手";
            _translations["RealTimeWeatherSystem"] = "实时天气查询系统";
            _translations["SearchCityPlaceholder"] = "输入城市名称（中文或英文）";
            _translations["EnterCityName"] = "输入城市名称（中文或英文）";
            _translations["Search"] = "搜索";
            _translations["SearchCity"] = "搜索城市";
            _translations["CurrentCity"] = "当前城市";
            _translations["LastUpdate"] = "最后更新: {0}";
            _translations["CacheTime"] = "缓存时间: {0}（离线模式）";
            _translations["MinutesAgo"] = "分钟前";
            _translations["Now"] = "现在";
            _translations["Temperature"] = "温度";
            _translations["Condition"] = "天气状况";
            _translations["FeelsLike"] = "体感温度";
            _translations["Humidity"] = "湿度";
            _translations["WindSpeed"] = "风速";
            _translations["WindDirection"] = "风向";
            _translations["Pressure"] = "气压";
            _translations["Visibility"] = "能见度";
            _translations["UVIndex"] = "紫外线指数";
            _translations["Refresh"] = "刷新";
            _translations["RefreshWeather"] = "刷新天气";
            _translations["DetailedWeather"] = "详细天气";
            _translations["CityManagement"] = "城市管理";
            _translations["Settings"] = "设置";
            _translations["About"] = "关于";
            _translations["PleaseEnterCityName"] = "请输入城市名称";
            _translations["Initializing"] = "正在初始化...";
            _translations["RefreshingWeather"] = "正在刷新天气...";
            _translations["SearchingWeather"] = "正在查询 {0} 的天气...";
            _translations["RefreshFailed"] = "刷新失败: {0}";
            _translations["SearchFailed"] = "查询失败: {0}";
            _translations["QueryingWeather"] = "正在查询 {0} 的天气...";
            _translations["QueryFailed"] = "查询失败: {0}";
            _translations["RequestTimeout"] = "请求超时，请检查网络";
            _translations["NetworkRequestFailed"] = "网络请求失败";
            _translations["DataParsingFailed"] = "数据解析失败";
            _translations["NetworkErrorTryCache"] = "网络请求失败，尝试加载缓存";
            _translations["NoWeatherData"] = "暂无天气数据";
            _translations["WeatherDataUpdated"] = "天气数据已更新";
            _translations["NoCacheData"] = "暂无缓存数据。";
            _translations["LoadCacheFailed"] = "加载缓存失败。";

            // 城市管理界面
            _translations["SavedCities"] = "已保存的城市";
            _translations["NoSavedCities"] = "暂无保存的城市";
            _translations["AddCityHint"] = "在主界面搜索城市后可添加到收藏";
            _translations["Delete"] = "删除";
            _translations["ViewWeather"] = "查看天气";
            _translations["CityDeleted"] = "城市已删除";
            _translations["FailedToDeleteCity"] = "删除城市失败";
            _translations["ManageCities"] = "管理你关注的城市天气";
            _translations["RefreshList"] = "刷新列表";
            _translations["CitiesHint"] = "提示：在首页查询城市天气后会自动保存到此列表";
            _translations["LoadingCities"] = "正在加载城市列表...";
            _translations["CitiesLoaded"] = "已加载 {0} 个城市";
            _translations["DeleteFailed"] = "删除城市失败: {0}";
            _translations["SelectFailed"] = "选择失败: {0}";

            // 设置界面
            _translations["Appearance"] = "外观";
            _translations["DarkMode"] = "深色模式";
            _translations["Enable"] = "启用";
            _translations["Language"] = "语言";
            _translations["InterfaceLanguage"] = "界面语言";
            _translations["SettingsNote"] = "注：某些设置可能需要重启应用生效";
            _translations["SettingsSaved"] = "设置已保存";
            _translations["DarkThemeSaved"] = "深色主题设置已保存: {0}";
            _translations["LanguageSaved"] = "语言设置已保存: {0}";
            _translations["SaveSettingsFailed"] = "保存设置失败";

            // 关于界面
            _translations["AppName"] = "天气预报助手";
            _translations["Version"] = "v1.0.0";
            _translations["Description"] = "基于 Avalonia MVVM 的跨平台天气预报应用";
            _translations["Features"] = "功能特性";
            _translations["RealTimeWeatherQuery"] = "实时天气查询";
            _translations["LocalDataCache"] = "本地数据缓存";
            _translations["MultiCityManagement"] = "多城市管理";
            _translations["OfflineMode"] = "离线模式支持";
            _translations["CrossPlatformSupport"] = "跨平台支持（Windows/Linux/macOS）";
            _translations["TechStack"] = "技术栈";
            _translations["AvaloniaUI"] = "Avalonia UI 11.3.4";
            _translations["DotNet"] = ".NET 9.0";
            _translations["MVVMPattern"] = "MVVM 架构模式";
            _translations["Copyright"] = "© 2024 天气预报助手. All rights reserved.";

            // 初始化界面
            _translations["InitializingApp"] = "正在初始化应用...";
            _translations["LoadingResources"] = "正在加载资源，请稍候...";

            // 天气详情界面
            _translations["WeatherDetail"] = "天气详情";
            _translations["LoadingWeatherDetail"] = "正在加载详细天气信息...";
            _translations["ShowingData"] = "显示 {0} 的天气数据";
            _translations["LoadFailed"] = "加载失败: {0}";
            _translations["CacheData"] = "显示缓存数据（离线模式）";
            _translations["Tip"] = "提示";
            _translations["Tip1"] = "• 数据来源于 WeatherAPI.com";
            _translations["Tip2"] = "• 点击刷新按钮获取最新天气数据";
            _translations["Tip3"] = "• 离线状态下将显示缓存的天气信息";
            _translations["DetailedWeatherData"] = "详细天气数据";
        }

        private void LoadEnglishTranslations()
        {
            // Main interface
            _translations["AppTitle"] = "Weather Forecast Assistant";
            _translations["WeatherAssistant"] = "Weather Forecast Assistant";
            _translations["NavigationMenu"] = "Navigation Menu";
            _translations["WeatherHome"] = "Weather Home";
            _translations["WeatherForecast"] = "Weather Forecast";
            _translations["RealTimeWeatherQuerySystem"] = "Real-time Weather Query System";
            _translations["Greeting"] = "Welcome to Weather Assistant";
            _translations["RealTimeWeatherSystem"] = "Real-time Weather Query System";
            _translations["SearchCityPlaceholder"] = "Enter city name (Chinese or English)";
            _translations["EnterCityName"] = "Enter city name (Chinese or English)";
            _translations["Search"] = "Search";
            _translations["SearchCity"] = "Search City";
            _translations["CurrentCity"] = "Current City";
            _translations["LastUpdate"] = "Last Update: {0}";
            _translations["CacheTime"] = "Cache Time: {0} (Offline Mode)";
            _translations["MinutesAgo"] = "minutes ago";
            _translations["Now"] = "Now";
            _translations["Temperature"] = "Temperature";
            _translations["Condition"] = "Condition";
            _translations["FeelsLike"] = "Feels Like";
            _translations["Humidity"] = "Humidity";
            _translations["WindSpeed"] = "Wind Speed";
            _translations["WindDirection"] = "Wind Direction";
            _translations["Pressure"] = "Pressure";
            _translations["Visibility"] = "Visibility";
            _translations["UVIndex"] = "UV Index";
            _translations["Refresh"] = "Refresh";
            _translations["RefreshWeather"] = "Refresh Weather";
            _translations["DetailedWeather"] = "Detailed Weather";
            _translations["CityManagement"] = "City Management";
            _translations["Settings"] = "Settings";
            _translations["About"] = "About";
            _translations["PleaseEnterCityName"] = "Please enter city name";
            _translations["Initializing"] = "Initializing...";
            _translations["RefreshingWeather"] = "Refreshing weather...";
            _translations["SearchingWeather"] = "Searching weather for {0}...";
            _translations["RefreshFailed"] = "Refresh failed: {0}";
            _translations["SearchFailed"] = "Search failed: {0}";
            _translations["QueryingWeather"] = "Querying weather for {0}...";
            _translations["QueryFailed"] = "Query failed: {0}";
            _translations["RequestTimeout"] = "Request timeout, please check network";
            _translations["NetworkRequestFailed"] = "Network request failed";
            _translations["DataParsingFailed"] = "Data parsing failed";
            _translations["NetworkErrorTryCache"] = "Network request failed, trying to load cache";
            _translations["NoWeatherData"] = "No weather data available";
            _translations["WeatherDataUpdated"] = "Weather data updated";
            _translations["NoCacheData"] = "No cache data available.";
            _translations["LoadCacheFailed"] = "Failed to load cache.";

            // City management interface
            _translations["SavedCities"] = "Saved Cities";
            _translations["NoSavedCities"] = "No saved cities";
            _translations["AddCityHint"] = "Search for cities in the main interface to add to favorites";
            _translations["Delete"] = "Delete";
            _translations["ViewWeather"] = "View Weather";
            _translations["CityDeleted"] = "City deleted";
            _translations["FailedToDeleteCity"] = "Failed to delete city";
            _translations["ManageCities"] = "Manage your favorite city weather";
            _translations["RefreshList"] = "Refresh List";
            _translations["CitiesHint"] = "Tip: Cities will be automatically saved to this list after querying weather on the home page";
            _translations["LoadingCities"] = "Loading city list...";
            _translations["CitiesLoaded"] = "Loaded {0} cities";
            _translations["DeleteFailed"] = "Failed to delete city: {0}";
            _translations["SelectFailed"] = "Selection failed: {0}";

            // Settings interface
            _translations["Appearance"] = "Appearance";
            _translations["DarkMode"] = "Dark Mode";
            _translations["Enable"] = "Enable";
            _translations["Language"] = "Language";
            _translations["InterfaceLanguage"] = "Interface Language";
            _translations["SettingsNote"] = "Note: Some settings may require app restart to take effect";
            _translations["SettingsSaved"] = "Settings saved";
            _translations["DarkThemeSaved"] = "Dark theme setting saved: {0}";
            _translations["LanguageSaved"] = "Language setting saved: {0}";
            _translations["SaveSettingsFailed"] = "Failed to save settings";

            // About interface
            _translations["AppName"] = "Weather Forecast Assistant";
            _translations["Version"] = "v1.0.0";
            _translations["Description"] = "Cross-platform weather forecast app based on Avalonia MVVM";
            _translations["Features"] = "Features";
            _translations["RealTimeWeatherQuery"] = "Real-time Weather Query";
            _translations["LocalDataCache"] = "Local Data Cache";
            _translations["MultiCityManagement"] = "Multi-city Management";
            _translations["OfflineMode"] = "Offline Mode Support";
            _translations["CrossPlatformSupport"] = "Cross-platform Support (Windows/Linux/macOS)";
            _translations["TechStack"] = "Tech Stack";
            _translations["AvaloniaUI"] = "Avalonia UI 11.3.4";
            _translations["DotNet"] = ".NET 9.0";
            _translations["MVVMPattern"] = "MVVM Architecture Pattern";
            _translations["Copyright"] = "© 2024 Weather Forecast Assistant. All rights reserved.";

            // Initialization interface
            _translations["InitializingApp"] = "Initializing application...";
            _translations["LoadingResources"] = "Loading resources, please wait...";

            // Weather detail interface
            _translations["WeatherDetail"] = "Weather Detail";
            _translations["LoadingWeatherDetail"] = "Loading detailed weather information...";
            _translations["ShowingData"] = "Showing weather data for {0}";
            _translations["LoadFailed"] = "Load failed: {0}";
            _translations["CacheData"] = "Showing cached data (offline mode)";
            _translations["Tip"] = "Tips";
            _translations["Tip1"] = "• Data source from WeatherAPI.com";
            _translations["Tip2"] = "• Click refresh button to get latest weather data";
            _translations["Tip3"] = "• Cached weather information will be displayed in offline mode";
            _translations["DetailedWeatherData"] = "Detailed Weather Data";
        }
    }
}