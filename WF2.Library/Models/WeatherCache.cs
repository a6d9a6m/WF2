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
    public double FeelsLikeC { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CachedAt { get; set; }
    public bool IsFavorite { get; set; } = false;

    public static WeatherCache FromApiResponse(WeatherApiResponse response)
    {
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
            FeelsLikeC = 0,  // API 未提供此字段
            LastUpdated = DateTime.Now,  // 使用当前时间
            CachedAt = DateTime.Now,
            IsFavorite = false  // 默认不关注，需要手动设置
        };
    }
}