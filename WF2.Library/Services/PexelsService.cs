using System.Text.Json;
using Microsoft.Extensions.Logging;
using WF2.Library.Interfaces;
using WF2.Library.Models;

namespace WF2.Library.Services;

/// <summary>
/// Pexels API服务实现
/// </summary>
public class PexelsService : IPexelsService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string? _accessKey;
    private readonly ILogger<PexelsService>? _logger;
    
    // 简单的内存缓存，避免重复请求相同图片
    private static readonly Dictionary<string, string> _imageCache = new();
    private static readonly object _cacheLock = new();
    
    private const string BaseUrl = "https://api.pexels.com/v1";

    public PexelsService(ILogger<PexelsService>? logger = null)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _accessKey = LoadApiKey();
        
        Console.WriteLine($"[DEBUG] PexelsService构造函数: 已初始化，API密钥状态: {(!string.IsNullOrEmpty(_accessKey) ? "已加载" : "未加载")}");
    }

    private string? LoadApiKey()
    {
        try
        {
            // 尝试从appsettings.json加载API密钥
            string appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            Console.WriteLine($"[DEBUG] PexelsService.LoadApiKey: 尝试从路径加载appsettings.json: {appSettingsPath}");
            
            if (File.Exists(appSettingsPath))
            {
                string jsonContent = File.ReadAllText(appSettingsPath);
                Console.WriteLine($"[DEBUG] PexelsService.LoadApiKey: 成功读取appsettings.json内容: {jsonContent}");
                
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                if (doc.RootElement.TryGetProperty("Pexels", out JsonElement pexelsElement))
                {
                    Console.WriteLine("[DEBUG] PexelsService.LoadApiKey: 找到Pexels配置节点");
                    
                    if (pexelsElement.TryGetProperty("AccessKey", out JsonElement keyElement))
                    {
                        string? key = keyElement.GetString();
                        Console.WriteLine($"[DEBUG] PexelsService.LoadApiKey: 读取到AccessKey: {key?.Substring(0, Math.Min(10, key?.Length ?? 0))}...");
                        
                        // 验证不是占位符
                        if (!string.IsNullOrEmpty(key) && !key.Contains("YOUR_PEXELS"))
                        {
                            Console.WriteLine("[DEBUG] PexelsService.LoadApiKey: API密钥有效，已加载");
                            return key;
                        }
                        else
                        {
                            Console.WriteLine("[DEBUG] PexelsService.LoadApiKey: API密钥是占位符或为空，需要配置真实密钥");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[DEBUG] PexelsService.LoadApiKey: 未找到AccessKey属性");
                    }
                }
                else
                {
                    Console.WriteLine("[DEBUG] PexelsService.LoadApiKey: 未找到Pexels配置节点");
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] PexelsService.LoadApiKey: appsettings.json文件不存在");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] PexelsService.LoadApiKey: 读取Pexels配置失败: {ex.Message}");
        }

        return null;
    }

    public async Task<string?> GetWeatherBackgroundImageAsync(string weatherCondition)
    {
        Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 尝试根据天气条件获取Pexels背景图片: {weatherCondition}");
        
        if (string.IsNullOrWhiteSpace(weatherCondition))
        {
            Console.WriteLine("[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 天气条件为空，返回null");
            return null;
        }

        // 如果没有配置API Key，直接返回null
        if (string.IsNullOrEmpty(_accessKey))
        {
            Console.WriteLine("[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: Pexels API密钥未配置或无效，返回null");
            return null;
        }
        
        Console.WriteLine("[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: Pexels API密钥已配置，继续处理请求");

        // 检查缓存
        string cacheKey = $"weather_{weatherCondition.ToLowerInvariant()}";
        if (TryGetFromCache(cacheKey, out string? cachedUrl))
        {
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 从缓存中获取到天气背景图片: {cachedUrl}");
            return cachedUrl;
        }

        try
        {
            // 根据天气条件映射搜索关键词
            string query = MapWeatherToSearchQuery(weatherCondition);
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 天气条件 '{weatherCondition}' 映射到搜索关键词: {query}");

            // 发送请求 - 使用Authorization头
            string url = $"{BaseUrl}/search?query={Uri.EscapeDataString(query)}&per_page=1";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _accessKey);
            
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 正在请求Pexels天气图片: {url}");
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 使用API密钥: {_accessKey?.Substring(0, Math.Min(10, _accessKey?.Length ?? 0))}...");
            
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 请求头信息:");
            foreach (var header in _httpClient.DefaultRequestHeaders)
            {
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: {header.Key}: {string.Join(", ", header.Value)}");
            }

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 响应状态码: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: Pexels API请求失败: {response.StatusCode}, 响应: {errorContent}");
                return null;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: Pexels API响应: {jsonResponse}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 开始解析Pexels API响应");
            var searchResult = JsonSerializer.Deserialize<PexelsPhotoResponse>(jsonResponse, options);
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 解析完成，结果: {searchResult != null}");
            
            if (searchResult != null)
            {
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: TotalResults: {searchResult.TotalResults}");
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: Photos: {(searchResult.Photos != null ? searchResult.Photos.Count : 0)}");
            }

            if (searchResult?.Photos?.Count > 0)
            {
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 找到 {searchResult.Photos.Count} 张图片");
                
                var photo = searchResult.Photos[0];
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 第一张图片ID: {photo.Id}");
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 第一张图片URL: {photo.Url}");
                
                if (photo.Src != null)
                {
                    Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 第一张图片Src.Large: {photo.Src.Large}");
                    Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 第一张图片Src.Medium: {photo.Src.Medium}");
                }
                
                string? imageUrl = photo.Src?.Large;

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // 添加到缓存
                    AddToCache(cacheKey, imageUrl);
                    Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 成功获取天气背景图片: {weatherCondition}, URL: {imageUrl}");
                    return imageUrl;
                }
                else
                {
                    Console.WriteLine("[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: Pexels API返回的图片没有Large URL");
                }
            }
            else
            {
                Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: Pexels API未返回任何结果，天气条件: {weatherCondition}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] PexelsService.GetWeatherBackgroundImageAsync: 获取天气背景图片时出错: {weatherCondition}, 错误: {ex.Message}");
        }

        return null;
    }
    
    private static string MapWeatherToSearchQuery(string weatherCondition)
    {
        // 将天气条件映射到更通用的搜索关键词
        string query = weatherCondition.ToLowerInvariant() switch
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
        
        Console.WriteLine($"[DEBUG] PexelsService.MapWeatherToSearchQuery: 天气条件 '{weatherCondition}' 映射到搜索关键词 '{query}'");
        return query;
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