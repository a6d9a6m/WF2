using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WF2.Library.Services;

namespace WF2.Library.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private string _title = "设置";
    
    [ObservableProperty]
    private string _appearanceLabel = "外观";
    
    [ObservableProperty]
    private string _darkModeLabel = "深色模式";
    
    [ObservableProperty]
    private string _darkModeCheckBox = "启用";
    
    [ObservableProperty]
    private string _languageLabel = "语言";
    
    [ObservableProperty]
    private string _interfaceLanguageLabel = "界面语言";
    
    [ObservableProperty]
    private string _restartHint = "注：某些设置可能需要重启应用生效";

    [ObservableProperty]
    private bool _useDarkTheme = true;

    [ObservableProperty]
    private string _selectedLanguage = "中文";

    public List<string> AvailableLanguages { get; } = new() { "中文", "English" };

    public SettingsViewModel(ISettingsService settingsService, ILocalizationService localizationService)
    {
        _settingsService = settingsService;
        _localizationService = localizationService;
        LoadSettings();
        
        // 订阅语言变更事件
        _localizationService.LanguageChanged += (sender, e) => UpdateUIText();
    }

    private async void LoadSettings()
    {
        UseDarkTheme = await _settingsService.GetUseDarkThemeAsync();
        SelectedLanguage = await _settingsService.GetSelectedLanguageAsync();
    }

    partial void OnUseDarkThemeChanged(bool value)
    {
        _ = SaveUseDarkThemeAsync(value);
    }

    private async Task SaveUseDarkThemeAsync(bool value)
    {
        try
        {
            await _settingsService.SaveUseDarkThemeAsync(value);
            Console.WriteLine($"[INFO] 深色主题设置已保存: {(value ? "启用" : "禁用")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 保存深色主题设置失败: {ex.Message}");
        }
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        _ = SaveSelectedLanguageAsync(value);
        // 更新本地化服务语言
        _localizationService.SetLanguage(value);
    }

    private async Task SaveSelectedLanguageAsync(string value)
    {
        try
        {
            await _settingsService.SaveSelectedLanguageAsync(value);
            Console.WriteLine($"[INFO] {_localizationService.GetString("LanguageSaved")}", value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {_localizationService.GetString("SaveSettingsFailed")}: {ex.Message}");
        }
    }

    private void UpdateUIText()
    {
        Title = _localizationService.GetString("Settings");
        AppearanceLabel = _localizationService.GetString("Appearance");
        DarkModeLabel = _localizationService.GetString("DarkMode");
        DarkModeCheckBox = _localizationService.GetString("Enable");
        LanguageLabel = _localizationService.GetString("Language");
        InterfaceLanguageLabel = _localizationService.GetString("InterfaceLanguage");
        RestartHint = _localizationService.GetString("RestartHint");
    }

    [RelayCommand]
    private void SaveSettings()
    {
        // 保存设置逻辑
        Console.WriteLine(_localizationService.GetString("SettingsSaved"));
    }
}
