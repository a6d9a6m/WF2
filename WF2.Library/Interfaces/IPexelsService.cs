namespace WF2.Library.Interfaces;

/// <summary>
/// Pexels API服务接口
/// </summary>
public interface IPexelsService
{
    /// <summary>
    /// 根据天气条件获取背景图片URL
    /// </summary>
    /// <param name="weatherCondition">天气条件</param>
    /// <returns>图片URL</returns>
    Task<string?> GetWeatherBackgroundImageAsync(string weatherCondition);
}