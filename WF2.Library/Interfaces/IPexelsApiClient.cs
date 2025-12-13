using Refit;
using WF2.Library.Models;

namespace WF2.Library.Interfaces;

/// <summary>
/// Pexels API 的 Refit 客户端接口
/// </summary>
public interface IPexelsApiClient
{
    /// <summary>
    /// 搜索图片
    /// </summary>
    /// <param name="query">搜索关键词</param>
    /// <param name="perPage">每页数量</param>
    [Get("/v1/search")]
    Task<PexelsPhotoResponse> SearchPhotosAsync(
        [Query] string query,
        [Query("per_page")] int perPage = 1);
}
