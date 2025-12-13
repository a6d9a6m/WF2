using WF2.Library.Models;

namespace WF2.Library.Interfaces;

/// <summary>
/// 天气服务接口
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// 获取指定城市的天气数据
    /// </summary>
    Task<WeatherApiResponse?> GetWeatherAsync(string cityName);
}
