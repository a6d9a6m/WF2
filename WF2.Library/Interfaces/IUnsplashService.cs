namespace WF2.Library.Interfaces;

/// <summary>
/// Unsplash API服务接口
/// </summary>
public interface IUnsplashService
{
    /// <summary>
    /// 根据城市名称获取背景图片URL
    /// </summary>
    /// <param name="cityName">城市名称</param>
    /// <returns>图片URL</returns>
    Task<string?> GetCityBackgroundImageAsync(string cityName);
    
    /// <summary>
    /// 根据天气条件获取背景图片URL
    /// </summary>
    /// <param name="weatherCondition">天气条件</param>
    /// <returns>图片URL</returns>
    Task<string?> GetWeatherBackgroundImageAsync(string weatherCondition);
}