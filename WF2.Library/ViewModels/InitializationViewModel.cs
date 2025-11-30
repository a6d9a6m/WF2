using CommunityToolkit.Mvvm.ComponentModel;
using WF2.Library.Services;

namespace WF2.Library.ViewModels;

public partial class InitializationViewModel : ViewModelBase
{
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private bool _useDarkTheme = true;
    
    [ObservableProperty]
    private string _selectedLanguage = "中文";

    [ObservableProperty]
    private string _title = "天气预报助手";

    [ObservableProperty]
    private string _subtitle = "正在初始化应用...";

    [ObservableProperty]
    private string _status = "正在加载资源，请稍候...";

    public InitializationViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        
        // 订阅语言变更事件
        _localizationService.LanguageChanged += (sender, e) => UpdateUIText();
    }

    private void UpdateUIText()
    {
        Title = _localizationService.GetString("WeatherAssistant");
        Subtitle = _localizationService.GetString("InitializingApp");
        Status = _localizationService.GetString("LoadingResources");
    }
}