namespace WF2.Library.Helpers;

public static class WeatherIconHelper
{
    /// <summary>
    /// 根据天气状况文本返回对应的emoji图标
    /// </summary>
    public static string GetWeatherIcon(string conditionText)
    {
        if (string.IsNullOrWhiteSpace(conditionText))
            return "🌤️";

        var condition = conditionText.ToLower();

        // 晴天
        if (condition.Contains("sunny") || condition.Contains("clear") ||
            condition.Contains("晴"))
            return "☀️";

        // 多云
        if (condition.Contains("partly cloudy") || condition.Contains("多云"))
            return "⛅";

        // 阴天
        if (condition.Contains("cloudy") || condition.Contains("overcast") ||
            condition.Contains("阴"))
            return "☁️";

        // 雨天
        if (condition.Contains("rain") || condition.Contains("drizzle") ||
            condition.Contains("shower") || condition.Contains("雨"))
        {
            if (condition.Contains("heavy") || condition.Contains("暴雨"))
                return "🌧️";
            if (condition.Contains("thunder") || condition.Contains("雷"))
                return "⛈️";
            return "🌦️";
        }

        // 雪天
        if (condition.Contains("snow") || condition.Contains("blizzard") ||
            condition.Contains("雪"))
            return "❄️";

        // 雾霾
        if (condition.Contains("fog") || condition.Contains("mist") ||
            condition.Contains("haze") || condition.Contains("雾") ||
            condition.Contains("霾"))
            return "🌫️";

        // 雷暴
        if (condition.Contains("thunder") || condition.Contains("storm") ||
            condition.Contains("雷"))
            return "⛈️";

        // 风
        if (condition.Contains("windy") || condition.Contains("风"))
            return "💨";

        // 默认
        return "🌤️";
    }

    /// <summary>
    /// 根据温度返回颜色（用于显示不同的温度等级）
    /// </summary>
    public static string GetTemperatureColor(double temperature)
    {
        return temperature switch
        {
            >= 35 => "#e74c3c",  // 极热 - 红色
            >= 30 => "#e67e22",  // 很热 - 橙色
            >= 25 => "#f39c12",  // 热 - 黄橙色
            >= 20 => "#27ae60",  // 温暖 - 绿色
            >= 15 => "#3498db",  // 凉爽 - 蓝色
            >= 10 => "#2980b9",  // 冷 - 深蓝色
            >= 0 => "#8e44ad",   // 很冷 - 紫色
            _ => "#2c3e50"       // 极冷 - 深灰色
        };
    }

    /// <summary>
    /// 根据UV指数返回提示文本
    /// </summary>
    public static string GetUvIndexDescription(double uvIndex)
    {
        return uvIndex switch
        {
            >= 11 => "极强 ⚠️",
            >= 8 => "很强 ☀️",
            >= 6 => "强 🌞",
            >= 3 => "中等 ⛅",
            _ => "弱 ☁️"
        };
    }

    /// <summary>
    /// 根据湿度返回描述
    /// </summary>
    public static string GetHumidityDescription(int humidity)
    {
        return humidity switch
        {
            >= 80 => "非常潮湿",
            >= 60 => "潮湿",
            >= 40 => "舒适",
            >= 20 => "干燥",
            _ => "非常干燥"
        };
    }
}
