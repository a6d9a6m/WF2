using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WF2.Library.Services;

namespace WF2.Library.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMenuNavigationService _menuNavigationService;
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;

    private ViewModelBase _content;

    public ViewModelBase Content {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    [ObservableProperty]
    private bool _useDarkTheme = true;

    [ObservableProperty]
    private string _selectedLanguage = "中文";

    // 添加本地化文本属性
    [ObservableProperty]
    private string _title = "天气预报助手";

    [ObservableProperty]
    private string _navigationMenu = "导航菜单";

    [ObservableProperty]
    private string _weatherHome = "天气首页";

    [ObservableProperty]
    private string _weatherDetail = "天气详情";

    [ObservableProperty]
    private string _cityManagement = "城市管理";

    [ObservableProperty]
    private string _settings = "设置";

    [ObservableProperty]
    private string _about = "关于";

    public MainWindowViewModel(IMenuNavigationService menuNavigationService, ISettingsService settingsService, ILocalizationService localizationService)
    {
        _menuNavigationService = menuNavigationService;
        _settingsService = settingsService;
        _localizationService = localizationService;
        _ = LoadSettingsAsync();
        
        // 订阅语言变更事件
        _localizationService.LanguageChanged += (sender, e) => UpdateUIText();
    }

    private async Task LoadSettingsAsync()
    {
        UseDarkTheme = await _settingsService.GetUseDarkThemeAsync();
        SelectedLanguage = await _settingsService.GetSelectedLanguageAsync();

        // 设置本地化服务的语言
        _localizationService.SetLanguage(SelectedLanguage);

        // 初始化UI文本
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        // 更新UI文本
        Title = _localizationService.GetString("WeatherAssistant");
        NavigationMenu = _localizationService.GetString("NavigationMenu");
        WeatherHome = _localizationService.GetString("WeatherHome");
        WeatherDetail = _localizationService.GetString("WeatherDetail");
        CityManagement = _localizationService.GetString("CityManagement");
        Settings = _localizationService.GetString("Settings");
        About = _localizationService.GetString("About");
    }

    [RelayCommand]
    private void NavigateToMain()
    {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.MainView);
    }

    [RelayCommand]
    private void NavigateToWeatherDetail()
    {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.WeatherDetailView);
    }

    [RelayCommand]
    private void NavigateToCities()
    {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.CitiesView);
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.SettingsView);
    }

    [RelayCommand]
    private void NavigateToAbout()
    {
        _menuNavigationService.NavigateTo(MenuNavigationConstant.AboutView);
    }
}
