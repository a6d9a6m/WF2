using Refit;
using WF2.Library.Models;

namespace WF2.Library.Interfaces;

/// <summary>
/// WeatherAPI.com 的 Refit 客户端接口
/// </summary>
public interface IWeatherApiClient
{
    /// <summary>
    /// 获取当前天气
    /// </summary>
    /// <param name="key">API密钥</param>
    /// <param name="q">城市名称</param>
    /// <param name="aqi">是否包含空气质量数据</param>
    [Get("/current.json")]
    Task<WeatherApiResponse> GetCurrentWeatherAsync(
        [Query] string key,
        [Query] string q,
        [Query] string aqi = "no");
}
