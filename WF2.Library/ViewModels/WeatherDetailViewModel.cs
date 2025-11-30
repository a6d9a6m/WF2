using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WF2.Library.Helpers;
using WF2.Library.Models;
using WF2.Library.Services;
using WF2.Library.Interfaces;
using Microsoft.Extensions.Configuration;

namespace WF2.Library.ViewModels;

public partial class WeatherDetailViewModel : ViewModelBase
{
    private const string ApiBaseUrl = "http://api.weatherapi.com/v1/current.json";
    private readonly IWeatherCacheService _cacheService;
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;
    private readonly IConfiguration _configuration;

    [ObservableProperty]
    private string _title = "天气详情";

    [ObservableProperty]
    private string _cityName = "Beijing";

    [ObservableProperty]
    private string _locationName = "";

    [ObservableProperty]
    private string _temperature = "--°C";

    [ObservableProperty]
    private string _conditionText = "--";

    [ObservableProperty]
    private string _humidity = "--%";

    [ObservableProperty]
    private double _windKph = 0;

    [ObservableProperty]
    private string _windDirection = "--";

    [ObservableProperty]
    private double _pressureMb = 0;

    [ObservableProperty]
    private double _visibilityKm = 0;

    [ObservableProperty]
    private double _uvIndex = 0;

    [ObservableProperty]
    private int _cloud = 0;

    [ObservableProperty]
    private string _feelsLike = "--°C";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private string _weatherIcon = "🌤️";

    [ObservableProperty]
    private bool _useDarkTheme = true;

    [ObservableProperty]
    private string _selectedLanguage = "中文";

    [ObservableProperty]
    private string _searchCityInput = "";

    [ObservableProperty]
    private bool _isFavorite = false;

    [ObservableProperty]
    private string _toastMessage = "";

    [ObservableProperty]
    private bool _showToast = false;

    // 添加本地化文本属性
    [ObservableProperty]
    private string _temperatureLabel = "温度";

    [ObservableProperty]
    private string _conditionLabel = "天气状况";

    [ObservableProperty]
    private string _humidityLabel = "湿度";

    [ObservableProperty]
    private string _windLabel = "风速";

    [ObservableProperty]
    private string _windDirectionLabel = "风向";

    [ObservableProperty]
    private string _pressureLabel = "气压";

    [ObservableProperty]
    private string _visibilityLabel = "能见度";

    [ObservableProperty]
    private string _uvIndexLabel = "紫外线指数";

    [ObservableProperty]
    private string _cloudLabel = "云量";

    [ObservableProperty]
    private string _feelsLikeLabel = "体感温度";

    [ObservableProperty]
    private string _refreshButton = "刷新";

    [ObservableProperty]
    private string _loadingMessage = "正在加载详细天气信息...";

    [ObservableProperty]
    private string _showingDataMessage = "显示 {0} 的天气数据";

    [ObservableProperty]
    private string _lastUpdateMessage = "最后更新: {0}";

    [ObservableProperty]
    private string _loadFailedMessage = "加载失败: {0}";

    [ObservableProperty]
    private string _cacheDataMessage = "显示缓存数据（离线模式）";

    [ObservableProperty]
        private string _noCacheDataMessage = "暂无缓存数据";
        
        [ObservableProperty]
        private string _loadCacheFailedMessage = "加载缓存失败";
        
        [ObservableProperty]
        private string _tipLabel = "提示";
        
        [ObservableProperty]
        private string _tip1Text = "• 数据来源于 WeatherAPI.com";
        
        [ObservableProperty]
        private string _tip2Text = "• 点击刷新按钮获取最新天气数据";
        
        [ObservableProperty]
        private string _tip3Text = "• 离线状态下将显示缓存的天气信息";
        
        [ObservableProperty]
        private string _detailedWeatherDataLabel = "详细天气数据";

    public WeatherDetailViewModel(IWeatherCacheService cacheService, ISettingsService settingsService, ILocalizationService localizationService, IConfiguration configuration)
    {
        _cacheService = cacheService;
        _settingsService = settingsService;
        _localizationService = localizationService;
        _configuration = configuration;
        
        // 订阅语言变更事件
        _localizationService.LanguageChanged += (sender, e) => UpdateUIText();
        
        _ = LoadSettingsAsync();
        _ = LoadWeatherDetailAsync();
    }

    private async Task LoadSettingsAsync()
    {
        UseDarkTheme = await _settingsService.GetUseDarkThemeAsync();
        SelectedLanguage = await _settingsService.GetSelectedLanguageAsync();
        
        // 设置本地化服务的语言
        _localizationService.SetLanguage(SelectedLanguage);
    }

    private void UpdateUIText()
    {
        Title = _localizationService.GetString("WeatherDetail");
        TemperatureLabel = _localizationService.GetString("Temperature");
        ConditionLabel = _localizationService.GetString("Condition");
        HumidityLabel = _localizationService.GetString("Humidity");
        WindLabel = _localizationService.GetString("Wind");
        WindDirectionLabel = _localizationService.GetString("WindDirection");
        PressureLabel = _localizationService.GetString("Pressure");
        VisibilityLabel = _localizationService.GetString("Visibility");
        UvIndexLabel = _localizationService.GetString("UvIndex");
        CloudLabel = _localizationService.GetString("Cloud");
        FeelsLikeLabel = _localizationService.GetString("FeelsLike");
        RefreshButton = _localizationService.GetString("Refresh");
        LoadingMessage = _localizationService.GetString("LoadingWeatherDetail");
        ShowingDataMessage = _localizationService.GetString("ShowingData");
        LastUpdateMessage = _localizationService.GetString("LastUpdate");
        LoadFailedMessage = _localizationService.GetString("LoadFailed");
        CacheDataMessage = _localizationService.GetString("CacheData");
        NoCacheDataMessage = _localizationService.GetString("NoCacheData");
        LoadCacheFailedMessage = _localizationService.GetString("LoadCacheFailed");
        TipLabel = _localizationService.GetString("Tip");
        Tip1Text = _localizationService.GetString("Tip1");
        Tip2Text = _localizationService.GetString("Tip2");
        Tip3Text = _localizationService.GetString("Tip3");
        DetailedWeatherDataLabel = _localizationService.GetString("DetailedWeatherData");
        
        // 更新体感温度显示
        FeelsLike = $"{FeelsLikeLabel} {Temperature}";
        
        // 更新湿度标签
        UpdateHumidityLabel();
    }
    
    // 更新湿度标签
    private void UpdateHumidityLabel()
    {
        // 如果湿度值已经包含%，则不需要再次添加标签
        if (Humidity.Contains("%"))
        {
            // 已经是完整格式，不需要修改
            return;
        }
        
        // 如果只是数字，添加%
        var humidityValue = Humidity;
        if (!string.IsNullOrEmpty(humidityValue) && !humidityValue.Contains("%"))
        {
            Humidity = $"{humidityValue}%";
        }
    }

    // 接收来自城市管理页面的天气数据
    public void SetWeatherData(WeatherCache weatherData)
    {
        if (weatherData != null)
        {
            CityName = weatherData.CityName;
            LocationName = $"{weatherData.LocationName}, {weatherData.Country}";
            Temperature = $"{weatherData.Temperature:F1}°C";
            ConditionText = weatherData.ConditionText;
            Humidity = $"{weatherData.Humidity}%";
            WindKph = weatherData.WindKph;
            WeatherIcon = WeatherIconHelper.GetWeatherIcon(weatherData.ConditionText);
            StatusMessage = string.Format(ShowingDataMessage, weatherData.LocationName);
            
            // 更新体感温度显示
            FeelsLike = $"{FeelsLikeLabel} {Temperature}";
            
            // 保存为最后选择的城市
            _ = _settingsService.SaveLastSelectedCityAsync(weatherData.CityName);
        }
    }

    private async Task LoadWeatherDetailAsync()
    {
        IsLoading = true;
        StatusMessage = LoadingMessage;

        try
        {
            // 如果没有设置城市名称，尝试获取上次选择的城市
            if (string.IsNullOrEmpty(CityName))
            {
                var lastSelectedCity = await _settingsService.GetLastSelectedCityAsync();
                if (!string.IsNullOrEmpty(lastSelectedCity))
                {
                    CityName = lastSelectedCity;
                    Console.WriteLine($"[INFO] 天气详情页加载上次选择的城市: {lastSelectedCity}");
                }
            }

            // 如果仍然没有城市名称，使用默认值
            if (string.IsNullOrEmpty(CityName))
            {
                CityName = "Beijing";
            }

            await FetchWeatherDetailAsync(CityName);

            // 检查城市是否已收藏
            await CheckFavoriteStatusAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 加载天气详情失败: {ex.Message}");
            StatusMessage = string.Format(LoadFailedMessage, ex.Message);
            await LoadCachedWeatherAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task FetchWeatherDetailAsync(string cityName)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);

        // 从配置中获取API密钥
        var apiKey = _configuration["WeatherApi:ApiKey"] ?? throw new InvalidOperationException("Weather API key not configured");
        
        var requestUrl = $"{ApiBaseUrl}?key={apiKey}&q={cityName}&aqi=yes";
        // 安全地记录日志，不包含API密钥
        Console.WriteLine($"[DEBUG] 获取天气详情: {ApiBaseUrl}?key=***&q={cityName}&aqi=yes");

        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(jsonString);

            if (weatherData != null)
            {
                UpdateWeatherDetail(weatherData);
                StatusMessage = string.Format(LastUpdateMessage, DateTime.Now.ToString("HH:mm:ss"));

                // 先检查是否已存在，保留IsFavorite状态
                var existing = await _cacheService.GetWeatherAsync(cityName);

                var cacheData = WeatherCache.FromApiResponse(weatherData);

                // 如果已存在，保留关注状态
                if (existing != null)
                {
                    cacheData.IsFavorite = existing.IsFavorite;
                    cacheData.Id = existing.Id; // 保留ID以便更新
                    IsFavorite = existing.IsFavorite; // 更新UI状态
                }

                // 保存到缓存
                await _cacheService.SaveWeatherAsync(cacheData);

                Console.WriteLine($"[DEBUG] 天气数据已保存，关注状态: {cacheData.IsFavorite}");
            }
        }
        else
        {
            throw new HttpRequestException($"API returned {response.StatusCode}");
        }
    }

    private void UpdateWeatherDetail(WeatherApiResponse data)
    {
        LocationName = $"{data.Location.Name}, {data.Location.Country}";
        Temperature = $"{data.Current.TempC:F1}°C";
        ConditionText = data.Current.Condition.Text;
        Humidity = $"{data.Current.Humidity}%";
        WindKph = data.Current.WindKph;
        WindDirection = data.Current.WindDir ?? "--";
        PressureMb = data.Current.PressureMb;
        VisibilityKm = data.Current.VisKm;
        UvIndex = data.Current.Uv;
        Cloud = data.Current.Cloud;
        FeelsLike = $"{data.Current.TempC:F1}°C";
        WeatherIcon = WeatherIconHelper.GetWeatherIcon(data.Current.Condition.Text);
        
        // 更新体感温度显示
        FeelsLike = $"{FeelsLikeLabel} {Temperature}";
        
        // 更新湿度标签
        UpdateHumidityLabel();
    }

    private async Task LoadCachedWeatherAsync()
    {
        try
        {
            var cachedData = await _cacheService.GetWeatherAsync(CityName);

            if (cachedData != null)
            {
                LocationName = $"{cachedData.LocationName}, {cachedData.Country}";
                Temperature = $"{cachedData.Temperature:F1}°C";
                ConditionText = cachedData.ConditionText;
                Humidity = $"{cachedData.Humidity}%";
                WindKph = cachedData.WindKph;
                WeatherIcon = WeatherIconHelper.GetWeatherIcon(cachedData.ConditionText);
                StatusMessage = CacheDataMessage;
                
                // 更新体感温度显示
                FeelsLike = $"{FeelsLikeLabel} {Temperature}";
                
                // 更新湿度标签
                UpdateHumidityLabel();
            }
            else
            {
                StatusMessage = NoCacheDataMessage;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 读取缓存失败: {ex.Message}");
            StatusMessage = LoadCacheFailedMessage;
        }
    }

    [RelayCommand]
    private async Task RefreshWeatherAsync()
    {
        await LoadWeatherDetailAsync();
    }

    [RelayCommand]
    private async Task SearchCityAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchCityInput))
        {
            StatusMessage = "请输入城市名称";
            return;
        }

        if (IsLoading) return;

        IsLoading = true;
        StatusMessage = $"正在查询 {SearchCityInput} 的天气...";
        var searchCity = SearchCityInput;
        SearchCityInput = ""; // 清空搜索框

        try
        {
            CityName = searchCity;
            await FetchWeatherDetailAsync(searchCity);

            // 检查城市是否已收藏
            await CheckFavoriteStatusAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 搜索城市失败: {ex.Message}");
            StatusMessage = $"查询失败: {ex.Message}";
            await LoadCachedWeatherAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        try
        {
            // 检查是否已经关注
            if (IsFavorite)
            {
                ShowToastMessage($"{CityName} 已经关注过了");
                Console.WriteLine($"[INFO] 城市 {CityName} 已经关注过了");
                return;
            }

            // 添加关注
            IsFavorite = true;
            await _cacheService.UpdateFavoriteStatusAsync(CityName, true);
            Console.WriteLine($"[INFO] 城市 {CityName} 已添加关注");
            ShowToastMessage($"已成功关注 {CityName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 更新关注状态失败: {ex.Message}");
            ShowToastMessage($"操作失败: {ex.Message}");
            IsFavorite = false;
        }
    }

    private void ShowToastMessage(string message)
    {
        ToastMessage = message;
        ShowToast = true;

        // 3秒后自动隐藏Toast
        Task.Run(async () =>
        {
            await Task.Delay(3000);
            ShowToast = false;
        });
    }

    private async Task CheckFavoriteStatusAsync()
    {
        try
        {
            var weatherData = await _cacheService.GetWeatherAsync(CityName);
            IsFavorite = weatherData?.IsFavorite ?? false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 检查关注状态失败: {ex.Message}");
        }
    }

    // 设置城市名称
    public void SetCityName(string cityName)
    {
        CityName = cityName;
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
