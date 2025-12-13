using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System.IO;
using System.Net.Http;

namespace WF2.Converters;

public class ImagePathConverter : IValueConverter
{
    private static readonly HttpClient HttpClient = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrEmpty(path))
        {
            Console.WriteLine("[DEBUG] ImagePathConverter: 路径为空");
            return null;
        }

        try
        {
            Console.WriteLine($"[DEBUG] ImagePathConverter: 转换路径: {path}");

            // 1. 处理 avares:// 协议（Avalonia 资源）
            if (path.StartsWith("avares://"))
            {
                Console.WriteLine($"[DEBUG] ImagePathConverter: 使用 Avalonia 资源路径: {path}");
                return new Bitmap(Avalonia.Platform.AssetLoader.Open(new Uri(path)));
            }

            // 2. 处理 http:// 或 https:// 协议（网络图片）
            if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                Console.WriteLine($"[DEBUG] ImagePathConverter: 使用网络 URL: {path}");
                // 对于网络图片，返回 URL 字符串，让 Avalonia 的 Image 控件自己加载
                return new Bitmap(path);
            }

            // 3. 处理 file:// 协议
            if (path.StartsWith("file://"))
            {
                var localPath = new Uri(path).LocalPath;
                Console.WriteLine($"[DEBUG] ImagePathConverter: 转换 file:// 为本地路径: {localPath}");
                if (File.Exists(localPath))
                {
                    return new Bitmap(localPath);
                }
                Console.WriteLine($"[WARN] ImagePathConverter: 文件不存在: {localPath}");
                return null;
            }

            // 4. 处理本地文件路径
            if (File.Exists(path))
            {
                Console.WriteLine($"[DEBUG] ImagePathConverter: 使用本地文件路径: {path}");
                return new Bitmap(path);
            }

            Console.WriteLine($"[WARN] ImagePathConverter: 无法识别或文件不存在: {path}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ImagePathConverter: 转换失败: {ex.Message}");
            Console.WriteLine($"[ERROR] 错误堆栈: {ex.StackTrace}");
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
