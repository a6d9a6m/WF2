using System.Globalization;

namespace WF2.Library.Models;

public class WeatherCache
{
    public int Id { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string ConditionText { get; set; } = string.Empty;
    public string ConditionIcon { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public double WindKph { get; set; }
    public string? WindDirection { get; set; }
    public double PressureMb { get; set; }
    public double VisibilityKm { get; set; }
    public double UvIndex { get; set; }
    public int Cloud { get; set; }
    public double FeelsLikeC { get; set; }
    public long LastUpdated { get; set; }  // 改为Unix时间戳
    public long CachedAt { get; set; }    // 改为Unix时间戳
    public bool IsFavorite { get; set; } = false;

    // 辅助方法：将Unix时间戳转换为DateTime
    public DateTime LastUpdatedDateTime => DateTimeOffset.FromUnixTimeSeconds(LastUpdated).DateTime;
    public DateTime CachedAtDateTime => DateTimeOffset.FromUnixTimeSeconds(CachedAt).DateTime;

    // 辅助方法：格式化时间显示
    public string LastUpdatedFormatted => LastUpdatedDateTime.ToString("HH:mm:ss");
    public string CachedAtFormatted => CachedAtDateTime.ToString("HH:mm:ss");

    // 检查缓存是否过期（默认10分钟）
    public bool IsExpired(int expirationMinutes = 10)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return (now - CachedAt) > (expirationMinutes * 60);
    }

    public static WeatherCache FromApiResponse(WeatherApiResponse response)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return new WeatherCache
        {
            CityName = response.Location.Name,
            LocationName = response.Location.Name,
            Country = response.Location.Country,
            Temperature = response.Current.TempC,
            ConditionText = response.Current.Condition.Text,
            ConditionIcon = response.Current.Condition.Icon,
            Humidity = response.Current.Humidity,
            WindKph = response.Current.WindKph,
            WindDirection = response.Current.WindDir,
            PressureMb = response.Current.PressureMb,
            VisibilityKm = response.Current.VisKm,
            UvIndex = response.Current.Uv,
            Cloud = response.Current.Cloud,
            FeelsLikeC = 0,  // API 未提供此字段
            LastUpdated = now,  // 使用Unix时间戳
            CachedAt = now,     // 使用Unix时间戳
            IsFavorite = false  // 默认不关注，需要手动设置
        };
    }
}