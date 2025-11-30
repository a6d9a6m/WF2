using System.Text.Json.Serialization;
using WF2.Library.Interfaces;

namespace WF2.Library.Models;

/// <summary>
/// Unsplash API响应模型
/// </summary>
public class UnsplashPhotoResponse
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("results")]
    public List<UnsplashPhoto>? Results { get; set; }
}

/// <summary>
/// Unsplash照片模型
/// </summary>
public class UnsplashPhoto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("blur_hash")]
    public string? BlurHash { get; set; }

    [JsonPropertyName("likes")]
    public int Likes { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("alt_description")]
    public string? AltDescription { get; set; }

    [JsonPropertyName("urls")]
    public UnsplashUrls? Urls { get; set; }

    [JsonPropertyName("links")]
    public UnsplashLinks? Links { get; set; }

    [JsonPropertyName("user")]
    public UnsplashUser? User { get; set; }
}

/// <summary>
/// Unsplash照片URL模型
/// </summary>
public class UnsplashUrls
{
    [JsonPropertyName("raw")]
    public string? Raw { get; set; }

    [JsonPropertyName("full")]
    public string? Full { get; set; }

    [JsonPropertyName("regular")]
    public string? Regular { get; set; }

    [JsonPropertyName("small")]
    public string? Small { get; set; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }
}

/// <summary>
/// Unsplash照片链接模型
/// </summary>
public class UnsplashLinks
{
    [JsonPropertyName("self")]
    public string? Self { get; set; }

    [JsonPropertyName("html")]
    public string? Html { get; set; }

    [JsonPropertyName("download")]
    public string? Download { get; set; }
}

/// <summary>
/// Unsplash用户模型
/// </summary>
public class UnsplashUser
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}