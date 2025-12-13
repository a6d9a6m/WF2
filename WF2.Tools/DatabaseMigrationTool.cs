using LiteDB;
using WF2.Library.Models;

namespace WF2.Tools;

public class DatabaseMigrationTool
{
    private const string DatabasePath = "Filename=weather.db;Connection=shared";
    private const string CollectionName = "weather_cache";

    public static void MigrateDatabase()
    {
        try
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<WeatherCache>(CollectionName);
            
            // 获取所有记录
            var allRecords = collection.FindAll().ToList();
            
            Console.WriteLine($"Found {allRecords.Count} records in weather_cache collection");
            
            int updatedCount = 0;
            foreach (var record in allRecords)
            {
                bool needsUpdate = false;
                
                // 检查LastUpdated字段是否需要修复
                if (record.LastUpdated == 0)
                {
                    // 如果LastUpdated为0，设置为当前时间戳
                    record.LastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    needsUpdate = true;
                    Console.WriteLine($"Fixed LastUpdated for {record.CityName}");
                }
                
                // 检查CachedAt字段是否需要修复
                if (record.CachedAt == 0)
                {
                    // 如果CachedAt为0，设置为当前时间戳
                    record.CachedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    needsUpdate = true;
                    Console.WriteLine($"Fixed CachedAt for {record.CityName}");
                }
                
                if (needsUpdate)
                {
                    collection.Update(record);
                    updatedCount++;
                }
            }
            
            Console.WriteLine($"Migration completed. Updated {updatedCount} records.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during migration: {ex.Message}");
        }
    }
}