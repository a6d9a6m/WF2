using System;
using WF2.Library.Services;

namespace WF2.Services;

public class RootNavigationService : IRootNavigationService
{
    public void NavigateTo(string view)
    {
        ServiceLocator.Current.MainWindowViewModel.Content = view switch
        {
            RootNavigationConstant.InitializationView => ServiceLocator.Current.InitializationViewModel,
            
            RootNavigationConstant.MainView => ServiceLocator.Current.MainViewModel,

            _ => throw new Exception("unknow view")
        };
    }
}