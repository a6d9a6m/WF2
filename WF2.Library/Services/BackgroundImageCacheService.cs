using LiteDB;
using WF2.Library.Models;

namespace WF2.Library.Services;

public interface IBackgroundImageCacheService
{
    Task<BackgroundImageCache?> GetImageByUrlAsync(string imageUrl);
    Task SaveImageAsync(BackgroundImageCache image);
    Task<List<BackgroundImageCache>> GetAllImagesAsync();
    Task DeleteImageAsync(int id);
    Task LimitCacheSizeAsync(int maxImages);
    Task DeleteExpiredCacheAsync(int maxAgeDays);
}

public class BackgroundImageCacheService : IBackgroundImageCacheService
{
    private const string DatabasePath = "Filename=weather.db;Connection=shared";
    private const string CollectionName = "background_image_cache";

    private LiteDatabase GetDatabase()
    {
        return new LiteDatabase(DatabasePath);
    }

    public async Task<BackgroundImageCache?> GetImageByUrlAsync(string imageUrl)
    {
        return await Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<BackgroundImageCache>(CollectionName);
            
            // 生成URL哈希
            var urlHash = ComputeHash(imageUrl);
            var image = collection.FindOne(x => x.UrlHash == urlHash);
            
            if (image != null)
            {
                // 更新最后访问时间
                image.LastAccessed = DateTime.Now;
                collection.Update(image);
            }
            
            return image;
        });
    }

    public async Task SaveImageAsync(BackgroundImageCache image)
    {
        await Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<BackgroundImageCache>(CollectionName);
            
            // 检查是否已存在相同URL的图片
            var existing = collection.FindOne(x => x.UrlHash == image.UrlHash);
            
            if (existing != null)
            {
                // 更新现有数据
                image.Id = existing.Id;
                image.LastAccessed = DateTime.Now;
                collection.Update(image);
            }
            else
            {
                // 插入新数据
                collection.Insert(image);
            }
            
            collection.EnsureIndex(x => x.UrlHash);
            collection.EnsureIndex(x => x.LastAccessed);
        });
    }

    public async Task<List<BackgroundImageCache>> GetAllImagesAsync()
    {
        return await Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<BackgroundImageCache>(CollectionName);
            return collection.FindAll().ToList();
        });
    }

    public async Task DeleteImageAsync(int id)
    {
        await Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<BackgroundImageCache>(CollectionName);
            collection.Delete(id);
        });
    }

    public async Task LimitCacheSizeAsync(int maxImages)
    {
        await Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<BackgroundImageCache>(CollectionName);
            
            // 获取所有图片，按最后访问时间排序（最近访问的在前面）
            var allImages = collection.FindAll().OrderByDescending(x => x.LastAccessed).ToList();
            
            // 如果图片数量超过限制，删除最旧的图片
            if (allImages.Count > maxImages)
            {
                var imagesToDelete = allImages.Skip(maxImages);
                foreach (var image in imagesToDelete)
                {
                    collection.Delete(image.Id);
                }
            }
        });
    }

    public async Task DeleteExpiredCacheAsync(int maxAgeDays)
    {
        await Task.Run(() =>
        {
            using var db = GetDatabase();
            var collection = db.GetCollection<BackgroundImageCache>(CollectionName);
            
            var expirationDate = DateTime.Now.AddDays(-maxAgeDays);
            collection.DeleteMany(x => x.LastAccessed < expirationDate);
        });
    }
    
    /// <summary>
    /// 计算字符串的SHA256哈希值
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>哈希值</returns>
    private static string ComputeHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash).Replace("/", "_").Replace("+", "-").Substring(0, 16);
    }
}