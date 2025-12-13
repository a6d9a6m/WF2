using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WF2.Library.Helpers;
using WF2.Library.Models;
using WF2.Library.Services;
using WF2.Library.Interfaces;
using Microsoft.Extensions.Configuration;

namespace WF2.Library.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const string ApiBaseUrl = "http://api.weatherapi.com/v1/current.json";
    private readonly IWeatherCacheService _cacheService;
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;
    private readonly IBackgroundImageService _backgroundImageService;
    private readonly IConfiguration _configuration;

    private ViewModelBase? _content;

    public ViewModelBase? Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    [ObservableProperty]
    private string _locationName = "正在初始化...";

    [ObservableProperty]
    private string _temperature = "--°C";

    [ObservableProperty]
    private string _conditionText = "--";

    [ObservableProperty]
    private string _humidity = "湿度: --%";

    [ObservableProperty]
    private string _statusMessage = "正在加载天气数据...";

    [ObservableProperty]
    private string _greeting = "天气预报";

    [ObservableProperty]
    private string _systemSubtitle = "实时天气查询系统";

    [ObservableProperty]
    private string _searchWatermark = "输入城市名称（中文或英文）";

    [ObservableProperty]
    private string _searchButton = "搜索";

    [ObservableProperty]
    private string _feelsLikeLabel = "体感温度";

    [ObservableProperty]
    private string _refreshButton = "刷新天气";

    [ObservableProperty]
    private string _initializingMessage = "正在初始化...";

    [ObservableProperty]
    private string _refreshingMessage = "正在刷新天气...";

    [ObservableProperty]
    private string _searchingMessage = "正在查询 {0} 的天气...";

    [ObservableProperty]
    private string _enterCityNameMessage = "请输入城市名称";

    [ObservableProperty]
    private string _refreshFailedMessage = "刷新失败: {0}";

    [ObservableProperty]
    private string _searchFailedMessage = "查询失败: {0}";

    [ObservableProperty]
    private string _queryingMessage = "正在查询 {0} 的天气...";

    [ObservableProperty]
    private string _queryFailedMessage = "查询失败: {0}";

    [ObservableProperty]
    private string _requestTimeoutMessage = "请求超时，请检查网络";

    [ObservableProperty]
    private string _networkRequestFailedMessage = "网络请求失败";

    [ObservableProperty]
    private string _dataParsingFailedMessage = "数据解析失败";

    [ObservableProperty]
    private string _lastUpdateMessage = "最后更新: {0}";

    [ObservableProperty]
    private string _cacheTimeMessage = "缓存时间: {0}（离线模式）";

    [ObservableProperty]
    private string _noCacheDataMessage = "暂无缓存数据。";

    [ObservableProperty]
    private string _loadCacheFailedMessage = "加载缓存失败。";

    [ObservableProperty]
    private string _currentCity = "Beijing";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private double _windKph = 0;

    [ObservableProperty]
    private string _feelsLike = "--°C";

    [ObservableProperty]
    private string _weatherIcon = "🌤️";

    [ObservableProperty]
    private bool _useDarkTheme = true;

    [ObservableProperty]
    private string _selectedLanguage = "中文";

    [ObservableProperty]
    private string _backgroundImagePath = string.Empty;

    public MainViewModel(IWeatherCacheService cacheService, ISettingsService settingsService,
        ILocalizationService localizationService, IBackgroundImageService backgroundImageService, IConfiguration configuration)
    {
        _cacheService = cacheService;
        _settingsService = settingsService;
        _localizationService = localizationService;
        _backgroundImageService = backgroundImageService;
        _configuration = configuration;

        // 订阅语言变更事件
        _localizationService.LanguageChanged += (sender, e) => UpdateUIText();

        _ = InitializeAsync();
    }

    public void PushContent(ViewModelBase content)
    {
        Content = content;
        // 页面切换时自动刷新天气数据
        _ = RefreshWeatherOnPageSwitchAsync();
    }

    // 页面切换时自动刷新天气
    public async Task RefreshWeatherOnPageSwitchAsync()
    {
        try
        {
            // 尝试获取有效的缓存数据（10分钟内）
            var validCache = await _cacheService.GetValidWeatherAsync(CurrentCity, 10);
            
            if (validCache != null)
            {
                // 有有效缓存，直接显示
                UpdateWeatherInfoFromCache(validCache);
                Console.WriteLine($"[INFO] 页面切换使用有效缓存数据: {validCache.CityName}");
            }
            else
            {
                // 无有效缓存，尝试获取实时数据
                await CheckWeatherAsync(CurrentCity);
                Console.WriteLine($"[INFO] 页面切换刷新天气数据: {CurrentCity}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] 页面切换刷新失败，尝试加载缓存：{ex.Message}");
            await LoadCachedWeatherAsync();
        }
    }

    private async Task RefreshWeatherAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        StatusMessage = RefreshingMessage;

        try
        {
            await CheckWeatherAsync(CurrentCity);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format(RefreshFailedMessage, ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    //初始化函数
    private async Task InitializeAsync()
    {
        try
        {
            // 加载设置
            UseDarkTheme = await _settingsService.GetUseDarkThemeAsync();
            SelectedLanguage = await _settingsService.GetSelectedLanguageAsync();

            // 加载背景图片路径
            BackgroundImagePath = await _backgroundImageService.GetBackgroundImagePathAsync();

            // 尝试获取上次选择的城市
            var lastSelectedCity = await _settingsService.GetLastSelectedCityAsync();
            if (!string.IsNullOrEmpty(lastSelectedCity))
            {
                CurrentCity = lastSelectedCity;
                Console.WriteLine($"[INFO] 加载上次选择的城市: {lastSelectedCity}");
            }

            // 初始化UI文本
            UpdateUIText();

            // 尝试获取有效的缓存数据（10分钟内）
            var validCache = await _cacheService.GetValidWeatherAsync(CurrentCity, 10);
            
            if (validCache != null)
            {
                // 有有效缓存，直接显示
                UpdateWeatherInfoFromCache(validCache);
                Console.WriteLine($"[INFO] 使用有效缓存数据: {validCache.CityName}");
            }
            else
            {
                // 无有效缓存，尝试获取实时数据
                await CheckWeatherAsync(CurrentCity);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] 网络请求失败，尝试加载缓存：{ex.Message}");
            await LoadCachedWeatherAsync();
        }
    }

    //查询天气
    private async Task CheckWeatherAsync(string cityName)
    {
        using var httpClient = new HttpClient();

        httpClient.Timeout = TimeSpan.FromSeconds(10);

        StatusMessage = $"正在查询 {cityName} 的天气...";

        try
        {
            // 从配置中获取API密钥
            var apiKey = _configuration["WeatherApi:ApiKey"] ?? throw new InvalidOperationException("Weather API key not configured");
            
            //构建请求 URL
            var requestUrl = $"{ApiBaseUrl}?key={apiKey}&q={cityName}&aqi=no";
            Console.WriteLine($"正在查询天气: {ApiBaseUrl}?key=***&q={cityName}&aqi=no");

            //发起 GET 请求
            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(jsonString);

                if (weatherData != null)
                {
                    UpdateWeatherInfo(weatherData);
                    await SaveWeatherCacheAsync(weatherData);
                    Console.WriteLine("[DEBUG] 天气数据已更新。");
                }
            }
            else
            {
                // 处理 API 错误
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API 请求失败: {response.StatusCode}");
                Console.WriteLine($"错误详情: {errorContent}");
                StatusMessage = $"查询失败: {response.StatusCode}";
                throw new HttpRequestException($"API returned {response.StatusCode}");
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            StatusMessage = RequestTimeoutMessage;
            Console.WriteLine("错误：HTTP 请求超时！");
            throw;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"网络请求错误: {ex.Message}");
            StatusMessage = NetworkRequestFailedMessage;
            throw;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON 解析错误: {ex.Message}");
            StatusMessage = DataParsingFailedMessage;
            throw;
        }
    }

    //  更新 UI 数据
    private void UpdateWeatherInfo(WeatherApiResponse data)
    {
        LocationName = $"{data.Location.Name}, {data.Location.Country}";
        Temperature = $"{data.Current.TempC:F1}°C";
        ConditionText = data.Current.Condition.Text;
        Humidity = $"湿度: {data.Current.Humidity}%";
        WindKph = data.Current.WindKph;
        FeelsLike = $"{FeelsLikeLabel} {data.Current.TempC:F1}°C"; // 更新体感温度显示
        WeatherIcon = WeatherIconHelper.GetWeatherIcon(data.Current.Condition.Text);
        StatusMessage = string.Format(LastUpdateMessage, DateTime.Now.ToString("HH:mm:ss"));
        CurrentCity = data.Location.Name;

        // 更新湿度标签
        UpdateHumidityLabel();

        // 根据天气条件更新背景图片
        _ = UpdateBackgroundImageAsync(data.Current.Condition.Text);
    }

    private void UpdateWeatherInfoFromCache(WeatherCache cache)
    {
        LocationName = $"{cache.LocationName}, {cache.Country}";
        Temperature = $"{cache.Temperature:F1}°C";
        ConditionText = cache.ConditionText;
        Humidity = $"湿度: {cache.Humidity}%";
        WindKph = cache.WindKph;
        FeelsLike = $"{FeelsLikeLabel} {cache.Temperature:F1}°C"; // 更新体感温度显示
        WeatherIcon = WeatherIconHelper.GetWeatherIcon(cache.ConditionText);
        StatusMessage = string.Format(CacheTimeMessage, cache.CachedAtFormatted);
        CurrentCity = cache.CityName;

        // 更新湿度标签
        UpdateHumidityLabel();
    }

    // 更新湿度标签
    private void UpdateHumidityLabel()
    {
        var humidityValue = Humidity.Contains(":") ? Humidity.Split(':')[1].Trim() : "--%";
        Humidity = $"{_localizationService.GetString("Humidity")}: {humidityValue}";
    }

    //存储到数据库
    private async Task SaveWeatherCacheAsync(WeatherApiResponse data)
    {
        try
        {
            // 先检查是否已存在，保留IsFavorite状态
            var existing = await _cacheService.GetWeatherAsync(data.Location.Name);

            var cache = WeatherCache.FromApiResponse(data);

            // 如果已存在，保留关注状态
            if (existing != null)
            {
                cache.IsFavorite = existing.IsFavorite;
                cache.Id = existing.Id; // 保留ID以便更新
            }

            await _cacheService.SaveWeatherAsync(cache);

            // 保存当前选择的城市
            await _settingsService.SaveLastSelectedCityAsync(data.Location.Name);

            Console.WriteLine($"[DEBUG] 天气数据已保存到数据库：{cache.CityName}, 关注状态: {cache.IsFavorite}");
            Console.WriteLine($"[DEBUG] 当前选择的城市已保存：{data.Location.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] 保存缓存失败：{ex.Message}");
        }
    }

    // 从数据库读取缓存
    private async Task LoadCachedWeatherAsync()
    {
        try
        {
            var cachedData = await _cacheService.GetWeatherAsync(CurrentCity);

            if (cachedData != null)
            {
                UpdateWeatherInfoFromCache(cachedData);
                // 添加缓存过期提示
                if (cachedData.IsExpired(10))
                {
                    StatusMessage += " (数据已过期)";
                }
            }
            else
            {
                StatusMessage = NoCacheDataMessage;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] 读取缓存失败：{ex.Message}");
            StatusMessage = LoadCacheFailedMessage;
        }
    }

    // 更新UI文本
    private void UpdateUIText()
    {
        Greeting = _localizationService.GetString("WeatherForecast");
        SystemSubtitle = _localizationService.GetString("RealTimeWeatherQuerySystem");
        SearchWatermark = _localizationService.GetString("EnterCityName");
        SearchButton = _localizationService.GetString("Search");
        FeelsLikeLabel = _localizationService.GetString("FeelsLike");
        RefreshButton = _localizationService.GetString("RefreshWeather");
        InitializingMessage = _localizationService.GetString("Initializing");
        RefreshingMessage = _localizationService.GetString("RefreshingWeather");
        SearchingMessage = _localizationService.GetString("SearchingWeather");
        EnterCityNameMessage = _localizationService.GetString("PleaseEnterCityName");
        RefreshFailedMessage = _localizationService.GetString("RefreshFailed");
        SearchFailedMessage = _localizationService.GetString("SearchFailed");
        QueryingMessage = _localizationService.GetString("QueryingWeather");
        QueryFailedMessage = _localizationService.GetString("QueryFailed");
        RequestTimeoutMessage = _localizationService.GetString("RequestTimeout");
        NetworkRequestFailedMessage = _localizationService.GetString("NetworkRequestFailed");
        DataParsingFailedMessage = _localizationService.GetString("DataParsingFailed");
        LastUpdateMessage = _localizationService.GetString("LastUpdate");
        CacheTimeMessage = _localizationService.GetString("CacheTime");
        NoCacheDataMessage = _localizationService.GetString("NoCacheData");
        LoadCacheFailedMessage = _localizationService.GetString("LoadCacheFailed");

        // 更新湿度标签
        UpdateHumidityLabel();
    }

    // 更新背景图片
    private async Task UpdateBackgroundImageAsync(string condition)
    {
        try
        {
            Console.WriteLine($"[DEBUG] MainViewModel.UpdateBackgroundImageAsync: 开始更新背景图片，天气条件: {condition}");
            var backgroundPath = await _backgroundImageService.GetBackgroundForWeatherConditionAsync(condition);
            Console.WriteLine($"[DEBUG] MainViewModel.UpdateBackgroundImageAsync: 获取到背景路径: {backgroundPath}");
            BackgroundImagePath = backgroundPath;
            Console.WriteLine($"[DEBUG] MainViewModel.UpdateBackgroundImageAsync: 背景图片路径已更新到ViewModel");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] MainViewModel.UpdateBackgroundImageAsync: 更新背景图片失败: {ex.Message}");
            Console.WriteLine($"[ERROR] 错误堆栈: {ex.StackTrace}");
        }
    }

}
