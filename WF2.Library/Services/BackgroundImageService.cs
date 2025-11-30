using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using WF2.Library.Interfaces;
using WF2.Library.Services;
using WF2.Library.Models;
using Microsoft.Extensions.Logging;

namespace WF2.Library.Services
{
    public class BackgroundImageService : IBackgroundImageService
    {
        private readonly IWeatherCacheService _weatherCacheService;
        private readonly ISettingsService _settingsService;
        private readonly IPexelsService _pexelsService;
        private readonly IBackgroundImageCacheService _backgroundImageCacheService;
        private readonly ILogger<BackgroundImageService>? _logger;
        private readonly string _lastUsedImageFile; // 存储上次使用图片路径的文件

        public BackgroundImageService(
            IWeatherCacheService weatherCacheService,
            ISettingsService settingsService,
            IPexelsService pexelsService,
            IBackgroundImageCacheService backgroundImageCacheService,
            ILogger<BackgroundImageService>? logger = null)
        {
            _weatherCacheService = weatherCacheService;
            _settingsService = settingsService;
            _pexelsService = pexelsService;
            _backgroundImageCacheService = backgroundImageCacheService;
            _logger = logger;
                
            _lastUsedImageFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WF2",
                "last_used_image.txt");

            // 确保目录存在
            var directory = Path.GetDirectoryName(_lastUsedImageFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public async Task<string> GetBackgroundImagePathAsync()
        {
            try
            {
                Console.WriteLine("[DEBUG] 开始获取背景图片路径...");

                // 首先尝试获取用户设置的背景图片
                var customBackgroundPath = await _settingsService.GetBackgroundImagePathAsync();
                if (!string.IsNullOrEmpty(customBackgroundPath) && (File.Exists(customBackgroundPath) || customBackgroundPath.StartsWith("avares://")))
                {
                    Console.WriteLine($"[DEBUG] 使用自定义背景图片: {customBackgroundPath}");
                    await SaveLastUsedImagePathAsync(customBackgroundPath);
                    return ConvertToUri(customBackgroundPath);
                }

                // 尝试获取上次使用的图片
                var lastUsedImagePath = GetLastUsedImagePath();
                if (!string.IsNullOrEmpty(lastUsedImagePath))
                {
                    if (lastUsedImagePath.StartsWith("http"))
                    {
                        Console.WriteLine($"[DEBUG] 使用上次缓存的网络图片URL: {lastUsedImagePath}");
                        return lastUsedImagePath;
                    }
                    else if (File.Exists(lastUsedImagePath))
                    {
                        Console.WriteLine($"[DEBUG] 使用上次缓存的本地背景图片: {lastUsedImagePath}");
                        return ConvertToUri(lastUsedImagePath);
                    }
                }

                // 尝试根据天气条件获取背景图片
                var lastSelectedCity = await _settingsService.GetLastSelectedCityAsync();
                if (!string.IsNullOrEmpty(lastSelectedCity))
                {
                    Console.WriteLine($"[DEBUG] 尝试为城市 '{lastSelectedCity}' 根据天气条件获取Pexels背景图片...");

                    var weatherData = await _weatherCacheService.GetWeatherAsync(lastSelectedCity);
                    if (weatherData != null)
                    {
                        Console.WriteLine($"[DEBUG] 当前天气条件: {weatherData.ConditionText}");
                        var weatherImageUrl = await _pexelsService.GetWeatherBackgroundImageAsync(weatherData.ConditionText);
                        if (!string.IsNullOrEmpty(weatherImageUrl))
                        {
                            Console.WriteLine($"[DEBUG] 成功获取天气背景图片URL: {weatherImageUrl}");

                            // 尝试从缓存获取或下载图片
                            var localImagePath = await GetOrDownloadImageAsync(weatherImageUrl, weatherData.ConditionText);
                            if (!string.IsNullOrEmpty(localImagePath))
                            {
                                Console.WriteLine($"[DEBUG] 使用本地缓存图片: {localImagePath}");
                                await SaveLastUsedImagePathAsync(localImagePath);
                                return ConvertToUri(localImagePath);
                            }

                            // 如果下载失败，直接使用网络URL
                            Console.WriteLine($"[DEBUG] 本地缓存失败，使用网络URL: {weatherImageUrl}");
                            await SaveLastUsedImagePathAsync(weatherImageUrl);
                            return weatherImageUrl;
                        }
                    }
                }

                // 默认背景
                Console.WriteLine("[DEBUG] 使用默认背景图片");
                return GetDefaultBackgroundPath();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "获取背景图片失败");
                Console.WriteLine($"[ERROR] 获取背景图片失败: {ex.Message}");
                return GetDefaultBackgroundPath();
            }
        }

        public async Task<string> GetBackgroundForWeatherConditionAsync(string condition)
        {
            try
            {
                Console.WriteLine($"[DEBUG] BackgroundImageService.GetBackgroundForWeatherConditionAsync: 尝试为天气条件 '{condition}' 获取背景图片");

                // 尝试从Pexels获取背景图片
                var pexelsUrl = await _pexelsService.GetWeatherBackgroundImageAsync(condition);

                if (!string.IsNullOrEmpty(pexelsUrl))
                {
                    Console.WriteLine($"[DEBUG] BackgroundImageService.GetBackgroundForWeatherConditionAsync: 成功获取Pexels背景图片URL: {pexelsUrl}");

                    // 尝试从缓存获取或下载图片
                    var localImagePath = await GetOrDownloadImageAsync(pexelsUrl, condition);
                    if (!string.IsNullOrEmpty(localImagePath))
                    {
                        Console.WriteLine($"[DEBUG] BackgroundImageService.GetBackgroundForWeatherConditionAsync: 使用本地缓存图片: {localImagePath}");
                        await SaveLastUsedImagePathAsync(localImagePath);
                        return ConvertToUri(localImagePath);
                    }

                    // 如果下载失败，直接使用网络URL
                    Console.WriteLine($"[DEBUG] 使用网络URL作为背景: {pexelsUrl}");
                    await SaveLastUsedImagePathAsync(pexelsUrl);
                    return pexelsUrl;
                }
                else
                {
                    Console.WriteLine("[DEBUG] BackgroundImageService.GetBackgroundForWeatherConditionAsync: Pexels返回空URL，使用本地默认背景");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] BackgroundImageService.GetBackgroundForWeatherConditionAsync: 获取Pexels背景图片失败: {ex.Message}");
            }

            // 如果Pexels服务失败，使用本地默认背景
            var localBackground = GetDefaultBackgroundPath();
            Console.WriteLine($"[DEBUG] BackgroundImageService.GetBackgroundForWeatherConditionAsync: 使用本地默认背景: {localBackground}");
            return localBackground;
        }

        public async Task<bool> SetCustomBackgroundAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                {
                    return false;
                }

                // 读取图片数据
                var imageData = await File.ReadAllBytesAsync(imagePath);
                
                // 保存到数据库缓存
                var backgroundImageCache = new BackgroundImageCache
                {
                    ImageUrl = "custom://" + Path.GetFileName(imagePath),
                    WeatherCondition = "custom",
                    ImageData = imageData,
                    UrlHash = ComputeHash("custom://" + Path.GetFileName(imagePath)),
                    LastAccessed = DateTime.Now,
                    CachedAt = DateTime.Now
                };
                
                await _backgroundImageCacheService.SaveImageAsync(backgroundImageCache);
                
                // 将图片数据保存到临时文件
                var tempImagePath = await SaveImageDataToTempFile(imageData);
                
                // 保存路径到设置
                await _settingsService.SaveBackgroundImagePathAsync(tempImagePath);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "设置自定义背景失败");
                return false;
            }
        }

        private string GetBackgroundForWeatherCondition(string condition)
        {
            // This method is no longer used - kept for backward compatibility
            return GetDefaultBackgroundPath();
        }

        private string GetDefaultBackgroundPath()
        {
            // 返回一个默认背景图片路径
            // 使用应用程序资源中的默认背景图片
            var defaultBackgroundPath = "avares://WF2/Assets/default-background.jpg";
            Console.WriteLine($"[DEBUG] 使用默认背景图片: {defaultBackgroundPath}");
            return defaultBackgroundPath;
        }

        /// <summary>
        /// 将Windows文件路径转换为URI格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>URI格式的路径</returns>
        private string ConvertToUri(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            // 如果已经是URI格式，直接返回
            if (filePath.StartsWith("http://") || filePath.StartsWith("https://") || filePath.StartsWith("file://") || filePath.StartsWith("avares://"))
                return filePath;

            // 将Windows路径转换为URI格式
            try
            {
                // 对于本地文件路径，使用完整的绝对URI
                var fullPath = Path.GetFullPath(filePath);
                var uri = new Uri(fullPath);
                var uriString = uri.AbsoluteUri;
                Console.WriteLine($"[DEBUG] 转换路径为URI: {filePath} -> {uriString}");
                return uriString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 转换路径为URI失败: {ex.Message}");
                return filePath;
            }
        }

        /// <summary>
        /// 从缓存获取图片或下载并缓存图片
        /// </summary>
        /// <param name="imageUrl">图片URL</param>
        /// <param name="weatherCondition">天气条件，用于生成缓存文件名</param>
        /// <returns>本地图片路径</returns>
        private async Task<string?> GetOrDownloadImageAsync(string imageUrl, string weatherCondition)
        {
            try
            {
                Console.WriteLine($"[DEBUG] 尝试从数据库缓存获取图片: {imageUrl}");
                
                // 尝试从数据库缓存获取图片
                var cachedImage = await _backgroundImageCacheService.GetImageByUrlAsync(imageUrl);
                if (cachedImage != null)
                {
                    Console.WriteLine($"[DEBUG] 从数据库缓存获取到图片: {imageUrl}");
                    
                    // 将图片数据保存到临时文件并返回路径
                    var tempImagePath = await SaveImageDataToTempFile(cachedImage.ImageData);
                    return tempImagePath;
                }

                // 从数据库缓存中未找到，下载图片
                Console.WriteLine($"[DEBUG] 开始下载图片: {imageUrl}");
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(imageUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR] 下载图片失败: {response.StatusCode}");
                    return null;
                }

                // 读取图片数据
                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await contentStream.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();
                
                Console.WriteLine($"[DEBUG] 图片下载成功，大小: {imageData.Length} 字节");
                
                // 保存到数据库缓存
                var urlHash = ComputeHash(imageUrl);
                var backgroundImageCache = new BackgroundImageCache
                {
                    ImageUrl = imageUrl,
                    WeatherCondition = weatherCondition,
                    ImageData = imageData,
                    UrlHash = urlHash,
                    LastAccessed = DateTime.Now,
                    CachedAt = DateTime.Now
                };
                
                await _backgroundImageCacheService.SaveImageAsync(backgroundImageCache);
                
                // 限制缓存大小为5张
                await _backgroundImageCacheService.LimitCacheSizeAsync(5);
                
                // 将图片数据保存到临时文件并返回路径
                var tempImagePath2 = await SaveImageDataToTempFile(imageData);
                Console.WriteLine($"[DEBUG] 图片已保存到临时文件: {tempImagePath2}");
                
                return tempImagePath2;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 下载或缓存图片失败: {ex.Message}");
                _logger?.LogError(ex, "下载或缓存图片失败");
                return null;
            }
        }
        
        /// <summary>
        /// 将图片数据保存到临时文件
        /// </summary>
        /// <param name="imageData">图片数据</param>
        /// <returns>临时文件路径</returns>
        private async Task<string> SaveImageDataToTempFile(byte[] imageData)
        {
            var tempDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WF2",
                "TempImages");
                
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
            
            var tempFileName = $"temp_{Guid.NewGuid()}.jpg";
            var tempFilePath = Path.Combine(tempDirectory, tempFileName);
            
            await File.WriteAllBytesAsync(tempFilePath, imageData);
            
            return ConvertToUri(tempFilePath);
        }

        /// <summary>
        /// 计算字符串的SHA256哈希值
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>哈希值</returns>
        private static string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash).Replace("/", "_").Replace("+", "-").Substring(0, 16);
        }

        /// <summary>
        /// 保存上次使用的图片路径
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        private async Task SaveLastUsedImagePathAsync(string imagePath)
        {
            try
            {
                Console.WriteLine($"[DEBUG] 保存上次使用的图片路径: {imagePath}");
                // 保存原始路径，不进行URI转换
                var originalPath = imagePath;
                if (imagePath.StartsWith("file://"))
                {
                    originalPath = new Uri(imagePath).LocalPath;
                }
                await File.WriteAllTextAsync(_lastUsedImageFile, originalPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 保存上次使用的图片路径失败: {ex.Message}");
                _logger?.LogError(ex, "保存上次使用的图片路径失败");
            }
        }

        /// <summary>
        /// 获取上次使用的图片路径
        /// </summary>
        /// <returns>上次使用的图片路径，如果不存在则返回null</returns>
        private string? GetLastUsedImagePath()
        {
            try
            {
                if (File.Exists(_lastUsedImageFile))
                {
                    var lastUsedPath = File.ReadAllText(_lastUsedImageFile);
                    Console.WriteLine($"[DEBUG] 获取上次使用的图片路径: {lastUsedPath}");
                    
                    // 检查文件是否仍然存在或者是网络URL
                    if (File.Exists(lastUsedPath))
                    {
                        // 返回URI格式的路径，以便UI可以正确显示
                        return ConvertToUri(lastUsedPath);
                    }
                    else if (lastUsedPath.StartsWith("http"))
                    {
                        return lastUsedPath;
                    }
                    else
                    {
                        Console.WriteLine($"[WARNING] 上次使用的图片文件不存在: {lastUsedPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 获取上次使用的图片路径失败: {ex.Message}");
                _logger?.LogError(ex, "获取上次使用的图片路径失败");
            }
            
            return null;
        }

        /// <summary>
        /// 限制缓存图片数量
        /// </summary>
        /// <param name="maxImages">最大图片数量</param>
        public async Task LimitCacheSizeAsync(int maxImages = 5)
        {
            try
            {
                Console.WriteLine($"[DEBUG] 限制数据库缓存大小，最大图片数: {maxImages}");
                await _backgroundImageCacheService.LimitCacheSizeAsync(maxImages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 限制数据库缓存大小失败: {ex.Message}");
                _logger?.LogError(ex, "限制数据库缓存大小失败");
            }
        }

        /// <summary>
        /// 清理过期的缓存图片
        /// </summary>
        /// <param name="maxAgeDays">最大缓存天数，默认为7天</param>
        public async Task CleanExpiredCacheAsync(int maxAgeDays = 7)
        {
            try
            {
                Console.WriteLine($"[DEBUG] 清理过期数据库缓存，最大天数: {maxAgeDays}");
                await _backgroundImageCacheService.DeleteExpiredCacheAsync(maxAgeDays);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 清理过期数据库缓存失败: {ex.Message}");
                _logger?.LogError(ex, "清理过期数据库缓存失败");
            }
        }

        public void Dispose()
        {
            // 不再需要释放_httpClient，因为我们现在在方法内部创建和使用它
        }
    }
}
