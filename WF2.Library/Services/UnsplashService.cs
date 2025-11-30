using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using WF2.Library.Interfaces;
using WF2.Library.Models;

namespace WF2.Library.Services;

/// <summary>
/// Unsplash API服务实现
/// </summary>
public class UnsplashService : IUnsplashService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UnsplashService>? _logger;
    private const string BaseUrl = "https://api.unsplash.com";

    // 从配置文件或环境变量读取Access Key
    // 如果未配置，将返回null，服务将优雅地降级到默认背景
    private readonly string? _accessKey;

    // 缓存字典，避免重复请求相同的图片
    private static readonly Dictionary<string, string> _imageCache = new();
    private static readonly object _cacheLock = new object();

    public UnsplashService(ILogger<UnsplashService>? logger = null)
    {
        _httpClient = new HttpClient();
        _logger = logger;

        // 尝试从配置文件读取API Key
        _accessKey = LoadAccessKeyFromConfig();

        if (!string.IsNullOrEmpty(_accessKey))
        {
            _logger?.LogInformation("Unsplash API已配置");
        }
        else
        {
            _logger?.LogWarning("Unsplash API Key未配置，将使用默认背景图片。请在appsettings.json中配置Unsplash.AccessKey");
        }
    }

    private string? LoadAccessKeyFromConfig()
    {
        try
        {
            // 尝试从appsettings.json读取
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                string jsonContent = File.ReadAllText(configPath);
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                if (doc.RootElement.TryGetProperty("Unsplash", out JsonElement unsplashElement))
                {
                    if (unsplashElement.TryGetProperty("AccessKey", out JsonElement keyElement))
                    {
                        string? key = keyElement.GetString();
                        // 验证不是占位符
                        if (!string.IsNullOrEmpty(key) && !key.Contains("YOUR_UNSPLASH"))
                        {
                            return key;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "读取Unsplash配置失败");
        }

        return null;
    }

    public async Task<string?> GetCityBackgroundImageAsync(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return null;

        // 如果没有配置API Key，直接返回null
        if (string.IsNullOrEmpty(_accessKey))
            return null;

        // 检查缓存
        string cacheKey = $"city_{cityName.ToLowerInvariant()}";
        if (TryGetFromCache(cacheKey, out string? cachedUrl))
            return cachedUrl;

        try
        {
            // 构建搜索查询
            string query = $"{cityName}";

            // 发送请求 - 使用 client_id 参数
            string url = $"{BaseUrl}/search/photos?query={Uri.EscapeDataString(query)}&client_id={_accessKey}&per_page=1&orientation=landscape";
            _logger?.LogInformation("正在请求Unsplash API: {Url}", url.Replace(_accessKey!, "***"));

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger?.LogWarning("Unsplash API请求失败: {StatusCode}, 响应: {Response}", response.StatusCode, errorContent);
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            _logger?.LogDebug("Unsplash API响应: {Response}", jsonResponse);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var searchResult = JsonSerializer.Deserialize<UnsplashPhotoResponse>(jsonResponse, options);

            if (searchResult?.Results?.Count > 0)
            {
                string? imageUrl = searchResult.Results[0].Urls?.Regular;

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // 添加到缓存
                    AddToCache(cacheKey, imageUrl);
                    _logger?.LogInformation("成功获取城市背景图片: {CityName}, URL: {ImageUrl}", cityName, imageUrl);
                    return imageUrl;
                }
                else
                {
                    _logger?.LogWarning("Unsplash API返回的图片没有Regular URL");
                }
            }
            else
            {
                _logger?.LogWarning("Unsplash API未返回任何结果，城市: {CityName}", cityName);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "获取城市背景图片时出错: {CityName}", cityName);
        }

        return null;
    }

    public async Task<string?> GetWeatherBackgroundImageAsync(string weatherCondition)
    {
        if (string.IsNullOrWhiteSpace(weatherCondition))
            return null;

        // 如果没有配置API Key，直接返回null
        if (string.IsNullOrEmpty(_accessKey))
            return null;

        // 检查缓存
        string cacheKey = $"weather_{weatherCondition.ToLowerInvariant()}";
        if (TryGetFromCache(cacheKey, out string? cachedUrl))
            return cachedUrl;

        try
        {
            // 根据天气条件映射搜索关键词
            string query = MapWeatherToSearchQuery(weatherCondition);

            // 发送请求 - 使用 client_id 参数
            string url = $"{BaseUrl}/search/photos?query={Uri.EscapeDataString(query)}&client_id={_accessKey}&per_page=1&orientation=landscape";
            _logger?.LogInformation("正在请求Unsplash天气图片: {Url}", url.Replace(_accessKey!, "***"));

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger?.LogWarning("Unsplash API请求失败: {StatusCode}, 响应: {Response}", response.StatusCode, errorContent);
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            _logger?.LogDebug("Unsplash API响应: {Response}", jsonResponse);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var searchResult = JsonSerializer.Deserialize<UnsplashPhotoResponse>(jsonResponse, options);

            if (searchResult?.Results?.Count > 0)
            {
                string? imageUrl = searchResult.Results[0].Urls?.Regular;

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // 添加到缓存
                    AddToCache(cacheKey, imageUrl);
                    _logger?.LogInformation("成功获取天气背景图片: {WeatherCondition}, URL: {ImageUrl}", weatherCondition, imageUrl);
                    return imageUrl;
                }
                else
                {
                    _logger?.LogWarning("Unsplash API返回的图片没有Regular URL");
                }
            }
            else
            {
                _logger?.LogWarning("Unsplash API未返回任何结果，天气条件: {WeatherCondition}", weatherCondition);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "获取天气背景图片时出错: {WeatherCondition}", weatherCondition);
        }

        return null;
    }
    
    private static string MapWeatherToSearchQuery(string weatherCondition)
    {
        // 将天气条件映射到更通用的搜索关键词
        return weatherCondition.ToLowerInvariant() switch
        {
            var s when s.Contains("sunny") || s.Contains("clear") => "clear sky sunny weather",
            var s when s.Contains("cloud") => "cloudy sky weather",
            var s when s.Contains("rain") => "rain weather storm",
            var s when s.Contains("snow") => "snow winter weather",
            var s when s.Contains("storm") => "thunderstorm storm weather",
            var s when s.Contains("fog") || s.Contains("mist") => "fog mist weather",
            var s when s.Contains("wind") => "windy weather",
            _ => "weather sky landscape"
        };
    }
    
    private static bool TryGetFromCache(string key, out string? value)
    {
        lock (_cacheLock)
        {
            return _imageCache.TryGetValue(key, out value);
        }
    }
    
    private static void AddToCache(string key, string value)
    {
        lock (_cacheLock)
        {
            _imageCache[key] = value;
        }
    }
    
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}