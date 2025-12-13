namespace WF2.Library.Services;

public static class ContentNavigationConstant
{
    // 具体页面内的内容导航
    public const string TodayDetail = "TodayDetail";
}

public interface IContentNavigationService
{
    void NavigateTo(string view, object? parameter = null);

    void GoBack();

    bool CanGoBack { get; }
}
