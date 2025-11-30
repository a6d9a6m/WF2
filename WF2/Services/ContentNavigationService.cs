using Microsoft.Extensions.DependencyInjection;
using WF2.Library.Services;
using WF2.Library.ViewModels;

namespace WF2.Services;

public class ContentNavigationService : IContentNavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<ViewModelBase> _navigationStack = new();

    public ContentNavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool CanGoBack => _navigationStack.Count > 1;

    public void NavigateTo(string view, object? parameter = null)
    {
        var content = view switch
        {
            ContentNavigationConstant.TodayDetail => ServiceLocator.Current.TodayDetailViewModel,
            _ => throw new Exception("Unknown view")
        };

        _navigationStack.Push(content);
        ServiceLocator.Current.MainViewModel.PushContent(content);
    }

    public void GoBack()
    {
        if (CanGoBack)
        {
            _navigationStack.Pop(); // 移除当前页面
            var previousPage = _navigationStack.Peek();
            ServiceLocator.Current.MainViewModel.PushContent(previousPage);
        }
    }
}
