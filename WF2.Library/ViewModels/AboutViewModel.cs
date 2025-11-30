using CommunityToolkit.Mvvm.ComponentModel;
using WF2.Library.Services;

namespace WF2.Library.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private string _title = "关于";

    [ObservableProperty]
    private string _appName = "天气预报助手";

    [ObservableProperty]
    private string _version = "v1.0.0";

    [ObservableProperty]
    private string _description = "基于 Avalonia MVVM 的跨平台天气预报应用";

    [ObservableProperty]
    private string _apiProvider = "WeatherAPI.com";
    
    [ObservableProperty]
    private bool _useDarkTheme = true;
    
    [ObservableProperty]
    private string _selectedLanguage = "中文";

    // 添加本地化文本属性
    [ObservableProperty]
    private string _features = "功能特性";

    [ObservableProperty]
    private string _feature1 = "✅ 实时天气查询";

    [ObservableProperty]
    private string _feature2 = "✅ 本地数据缓存";

    [ObservableProperty]
    private string _feature3 = "✅ 多城市管理";

    [ObservableProperty]
    private string _feature4 = "✅ 离线模式支持";

    [ObservableProperty]
    private string _feature5 = "✅ 跨平台支持（Windows/Linux/macOS）";

    [ObservableProperty]
    private string _techStack = "技术栈";

    [ObservableProperty]
    private string _tech1 = "🔹 Avalonia UI 11.3.4";

    [ObservableProperty]
    private string _tech2 = "🔹 .NET 9.0";

    [ObservableProperty]
    private string _tech3 = "🔹 MVVM 架构模式";

    [ObservableProperty]
    private string _tech4 = "🔹 LiteDB 数据库";

    [ObservableProperty]
    private string _copyright = "© 2025 天气预报助手";

    public AboutViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        
        // 订阅语言变更事件
        _localizationService.LanguageChanged += (sender, e) => UpdateUIText();
    }

    private void UpdateUIText()
    {
        Title = _localizationService.GetString("About");
        AppName = _localizationService.GetString("WeatherAssistant");
        Description = _localizationService.GetString("AppDescription");
        Features = _localizationService.GetString("Features");
        Feature1 = _localizationService.GetString("Feature1");
        Feature2 = _localizationService.GetString("Feature2");
        Feature3 = _localizationService.GetString("Feature3");
        Feature4 = _localizationService.GetString("Feature4");
        Feature5 = _localizationService.GetString("Feature5");
        TechStack = _localizationService.GetString("TechStack");
        Tech1 = _localizationService.GetString("Tech1");
        Tech2 = _localizationService.GetString("Tech2");
        Tech3 = _localizationService.GetString("Tech3");
        Tech4 = _localizationService.GetString("Tech4");
        Copyright = _localizationService.GetString("Copyright");
    }
}
