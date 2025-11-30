using System.Text.Json.Serialization;

namespace WF2.Library.Models;

/// <summary>
/// Pexels API响应模型
/// </summary>
public class PexelsPhotoResponse
{
    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("photos")]
    public List<PexelsPhoto>? Photos { get; set; }

    [JsonPropertyName("next_page")]
    public string? NextPage { get; set; }
}

/// <summary>
/// Pexels照片模型
/// </summary>
public class PexelsPhoto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("photographer")]
    public string? Photographer { get; set; }

    [JsonPropertyName("photographer_url")]
    public string? PhotographerUrl { get; set; }

    [JsonPropertyName("photographer_id")]
    public int PhotographerId { get; set; }

    [JsonPropertyName("avg_color")]
    public string? AvgColor { get; set; }

    [JsonPropertyName("src")]
    public PexelsSrc? Src { get; set; }

    [JsonPropertyName("liked")]
    public bool Liked { get; set; }

    [JsonPropertyName("alt")]
    public string? Alt { get; set; }
}

/// <summary>
/// Pexels照片源模型
/// </summary>
public class PexelsSrc
{
    [JsonPropertyName("original")]
    public string? Original { get; set; }

    [JsonPropertyName("large2x")]
    public string? Large2x { get; set; }

    [JsonPropertyName("large")]
    public string? Large { get; set; }

    [JsonPropertyName("medium")]
    public string? Medium { get; set; }

    [JsonPropertyName("small")]
    public string? Small { get; set; }

    [JsonPropertyName("portrait")]
    public string? Portrait { get; set; }

    [JsonPropertyName("landscape")]
    public string? Landscape { get; set; }

    [JsonPropertyName("tiny")]
    public string? Tiny { get; set; }
}