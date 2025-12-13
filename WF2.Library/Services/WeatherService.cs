using WF2.Library.Interfaces;
using WF2.Library.Models;

namespace WF2.Library.Services;

/// <summary>
/// 天气服务 - 使用Refit客户端
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly IWeatherApiClient _weatherApiClient;
    private readonly string? _apiKey;

    public WeatherService(IWeatherApiClient weatherApiClient)
    {
        _weatherApiClient = weatherApiClient;
        _apiKey = LoadApiKey();
    }

    private string? LoadApiKey()
    {
        try
        {
            string appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (File.Exists(appSettingsPath))
            {
                string jsonContent = File.ReadAllText(appSettingsPath);
                using var doc = System.Text.Json.JsonDocument.Parse(jsonContent);
                if (doc.RootElement.TryGetProperty("WeatherApi", out var weatherApiElement))
                {
                    if (weatherApiElement.TryGetProperty("ApiKey", out var keyElement))
                    {
                        return keyElement.GetString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WeatherService] 加载API密钥失败: {ex.Message}");
        }
        return null;
    }

    public async Task<WeatherApiResponse?> GetWeatherAsync(string cityName)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Console.WriteLine("[WeatherService] API密钥未配置");
            return null;
        }

        try
        {
            Console.WriteLine($"[WeatherService] 正在获取天气数据: {cityName}");
            var weather = await _weatherApiClient.GetCurrentWeatherAsync(_apiKey, cityName);
            Console.WriteLine($"[WeatherService] 成功获取天气数据: {cityName}");
            return weather;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WeatherService] 获取天气数据失败: {ex.Message}");
            return null;
        }
    }
}
