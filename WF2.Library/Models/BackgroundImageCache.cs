using LiteDB;

namespace WF2.Library.Models;

public class BackgroundImageCache
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string WeatherCondition { get; set; } = string.Empty;
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public DateTime LastAccessed { get; set; } = DateTime.Now;
    public DateTime CachedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// 从URL生成唯一标识符
    /// </summary>
    public string UrlHash { get; set; } = string.Empty;
}