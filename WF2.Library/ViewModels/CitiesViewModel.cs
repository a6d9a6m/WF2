using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WF2.Library.Models;
using WF2.Library.Services;

namespace WF2.Library.ViewModels;

public partial class CitiesViewModel : ViewModelBase
{
    private readonly IWeatherCacheService _cacheService;
    private readonly ISettingsService _settingsService;
    private readonly IMenuNavigationService _menuNavigationService;
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private string _title = "城市管理";
    
    [ObservableProperty]
    private string _subtitle = "管理你关注的城市天气";
    
    [ObservableProperty]
    private string _savedCitiesLabel = "已保存的城市";
    
    [ObservableProperty]
    private string _deleteButton = "删除";
    
    [ObservableProperty]
    private string _noCitiesMessage = "暂无保存的城市";
    
    [ObservableProperty]
    private string _hintMessage = "提示：在详情页查询城市并点击关注按钮后会自动保存到此列表";
    
    [ObservableProperty]
    private string _loadingCitiesMessage = "正在加载城市列表...";
    
    [ObservableProperty]
    private string _citiesLoadedMessage = "已加载 {0} 个城市";
    
    [ObservableProperty]
    private string _loadFailedMessage = "加载失败: {0}";
    
    [ObservableProperty]
    private string _deleteFailedMessage = "删除城市失败: {0}";
    
    [ObservableProperty]
    private string _selectFailedMessage = "选择失败: {0}";

    [ObservableProperty]
    private List<WeatherCache> _cities = new();

    [ObservableProperty]
    private string _newCityName = string.Empty;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _useDarkTheme = true;

    [ObservableProperty]
    private string _selectedLanguage = "中文";

    public CitiesViewModel(IWeatherCacheService cacheService, ISettingsService settingsService, IMenuNavigationService menuNavigationService, ILocalizationService localizationService)
    {
        _cacheService = cacheService;
        _settingsService = settingsService;
        _menuNavigationService = menuNavigationService;
        _localizationService = localizationService;
        
        // 订阅语言变更事件
        _localizationService.LanguageChanged += OnLanguageChanged;
        
        _ = InitializeAsync();
    }
    
    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateUIText();
    }
    
    private void UpdateUIText()
    {
        Title = _localizationService.GetString("CityManagement");
        Subtitle = _localizationService.GetString("ManageCities");
        SavedCitiesLabel = _localizationService.GetString("SavedCities");
        DeleteButton = _localizationService.GetString("Delete");
        NoCitiesMessage = _localizationService.GetString("NoSavedCities");
        HintMessage = _localizationService.GetString("CitiesHint");
        LoadingCitiesMessage = _localizationService.GetString("LoadingCities");
        CitiesLoadedMessage = _localizationService.GetString("CitiesLoaded");
        LoadFailedMessage = _localizationService.GetString("LoadFailed");
        DeleteFailedMessage = _localizationService.GetString("DeleteFailed");
        SelectFailedMessage = _localizationService.GetString("SelectFailed");
    }

    private async Task InitializeAsync()
    {
        // 加载设置
        UseDarkTheme = await _settingsService.GetUseDarkThemeAsync();
        SelectedLanguage = await _settingsService.GetSelectedLanguageAsync();
        
        // 加载城市列表
        await LoadCitiesAsync();
    }

    private async Task LoadCitiesAsync()
    {
        IsLoading = true;
        StatusMessage = LoadingCitiesMessage;

        try
        {
            // 只获取关注的城市
            var citiesList = await _cacheService.GetFavoriteCitiesAsync();

            // 确保citiesList不为null
            Cities = citiesList ?? new List<WeatherCache>();
            StatusMessage = string.Format(CitiesLoadedMessage, Cities.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 加载城市列表失败: {ex.Message}");
            StatusMessage = string.Format(LoadFailedMessage, ex.Message);
            
            // 确保在异常情况下Cities也不为null
            Cities = new List<WeatherCache>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    // 页面切换时自动刷新
    public async Task OnPageActivatedAsync()
    {
        await LoadCitiesAsync();
    }

    [RelayCommand]
    private async Task DeleteCityAsync(string cityName)
    {
        try
        {
            await _cacheService.DeleteWeatherAsync(cityName);
            await LoadCitiesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 删除城市失败: {ex.Message}");
            StatusMessage = string.Format(DeleteFailedMessage, ex.Message);
        }
    }

    [RelayCommand]
    private async Task SelectCityAsync(string cityName)
    {
        if (string.IsNullOrEmpty(cityName)) return;

        try
        {
            // 保存最后选择的城市
            await _settingsService.SaveLastSelectedCityAsync(cityName);
            
            // 获取城市天气数据
            var weatherData = await _cacheService.GetWeatherAsync(cityName);
            
            if (weatherData != null)
            {
                // 导航到天气详情页面
                _menuNavigationService.NavigateTo(MenuNavigationConstant.WeatherDetailView, weatherData);
            }
            else
            {
                // 如果没有缓存数据，只导航到天气详情页，让页面自己加载
                _menuNavigationService.NavigateTo(MenuNavigationConstant.WeatherDetailView);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 选择城市失败: {ex.Message}");
            StatusMessage = string.Format(SelectFailedMessage, ex.Message);
        }
    }

    // 更新语言
    public async Task UpdateLanguageAsync(string language)
    {
        try
        {
            // 设置本地化服务的语言
            _localizationService.SetLanguage(language);
            
            // 保存语言设置
            await _settingsService.SaveSelectedLanguageAsync(language);
            
            // 更新UI文本
            UpdateUIText();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 更新语言失败: {ex.Message}");
        }
    }
}
