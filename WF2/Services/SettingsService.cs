using LiteDB;
using WF2.Library.Services;

namespace WF2.Services;

public class SettingsService : ISettingsService
{
    private const string DatabasePath = "Filename=weather.db;Connection=shared";
    private const string SettingsCollectionName = "settings";

    public Task<bool> GetUseModernUIAsync()
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            var doc = collection.FindOne(d => d["Key"] == "UseModernUI");
            return doc?["Value"].AsBoolean ?? false;
        });
    }

    public Task SaveUseModernUIAsync(bool useModernUI)
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            
            // 删除已存在的记录
            collection.DeleteMany(d => d["Key"] == "UseModernUI");
            
            // 添加新记录
            var doc = new BsonDocument
            {
                ["Key"] = "UseModernUI",
                ["Value"] = useModernUI
            };
            collection.Insert(doc);
        });
    }

    public Task<string?> GetLastSelectedCityAsync()
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            var doc = collection.FindOne(d => d["Key"] == "LastSelectedCity");
            return doc?["Value"].AsString;
        });
    }

    public Task SaveLastSelectedCityAsync(string cityName)
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            
            // 删除已存在的记录
            collection.DeleteMany(d => d["Key"] == "LastSelectedCity");
            
            // 添加新记录
            var doc = new BsonDocument
            {
                ["Key"] = "LastSelectedCity",
                ["Value"] = cityName
            };
            collection.Insert(doc);
        });
    }

    public Task<bool> GetUseDarkThemeAsync()
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            var doc = collection.FindOne(d => d["Key"] == "UseDarkTheme");
            return doc?["Value"].AsBoolean ?? true; // 默认使用深色主题
        });
    }

    public Task SaveUseDarkThemeAsync(bool useDarkTheme)
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            
            // 删除已存在的记录
            collection.DeleteMany(d => d["Key"] == "UseDarkTheme");
            
            // 添加新记录
            var doc = new BsonDocument
            {
                ["Key"] = "UseDarkTheme",
                ["Value"] = useDarkTheme
            };
            collection.Insert(doc);
        });
    }

    public Task<string> GetSelectedLanguageAsync()
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            var doc = collection.FindOne(d => d["Key"] == "SelectedLanguage");
            return doc?["Value"].AsString ?? "中文"; // 默认使用中文
        });
    }

    public Task SaveSelectedLanguageAsync(string language)
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            
            // 删除已存在的记录
            collection.DeleteMany(d => d["Key"] == "SelectedLanguage");
            
            // 添加新记录
            var doc = new BsonDocument
            {
                ["Key"] = "SelectedLanguage",
                ["Value"] = language
            };
            collection.Insert(doc);
        });
    }

    public Task<string> GetBackgroundImagePathAsync()
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            var doc = collection.FindOne(d => d["Key"] == "BackgroundImagePath");
            return doc?["Value"].AsString ?? "avares://WF2/Assets/Background.axaml"; // 默认背景
        });
    }

    public Task SaveBackgroundImagePathAsync(string imagePath)
    {
        return Task.Run(() =>
        {
            using var db = new LiteDatabase(DatabasePath);
            var collection = db.GetCollection<BsonDocument>(SettingsCollectionName);
            
            // 删除已存在的记录
            collection.DeleteMany(d => d["Key"] == "BackgroundImagePath");
            
            // 添加新记录
            var doc = new BsonDocument
            {
                ["Key"] = "BackgroundImagePath",
                ["Value"] = imagePath
            };
            collection.Insert(doc);
        });
    }
}