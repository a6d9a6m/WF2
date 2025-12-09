using WF2.Library.Models;
using WF2.Library.Services;
using WF2.Library.ViewModels;

namespace WF2.Services;

public class MenuNavigationService : IMenuNavigationService
{
    public async void NavigateTo(string view, object? parameter = null)
    {
        ViewModelBase viewModel = view switch
        {
            MenuNavigationConstant.MainView => ServiceLocator.Current.MainViewModel,
            MenuNavigationConstant.WeatherDetailView => ServiceLocator.Current.WeatherDetailViewModel,
            MenuNavigationConstant.CitiesView => ServiceLocator.Current.CitiesViewModel,
            MenuNavigationConstant.SettingsView => ServiceLocator.Current.SettingsViewModel,
            MenuNavigationConstant.AboutView => ServiceLocator.Current.AboutViewModel,
            _ => throw new Exception("Unknown view")
        };

        // 如果导航到天气详情页面并且有参数，则设置天气数据
        if (view == MenuNavigationConstant.WeatherDetailView && parameter is WeatherCache weatherData)
        {
            ((WeatherDetailViewModel)viewModel).SetWeatherData(weatherData);
        }

        ServiceLocator.Current.MainWindowViewModel.Content = viewModel;
        
        // 页面切换时自动刷新
        switch (viewModel)
        {
            case MainViewModel mainViewModel:
                await mainViewModel.RefreshWeatherOnPageSwitchAsync();
                break;
            case WeatherDetailViewModel weatherDetailViewModel:
                await weatherDetailViewModel.OnPageActivatedAsync();
                break;
            case CitiesViewModel citiesViewModel:
                await citiesViewModel.OnPageActivatedAsync();
                break;
            // SettingsViewModel和AboutViewModel不需要自动刷新
        }
    }
}
