using System;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WF2.Library.Services;
using WF2.Library.ViewModels;
using WF2.Services;
using WF2.Library.Interfaces;

namespace WF2;

public class ServiceLocator
{
    private readonly IServiceProvider _serviceProvider;

    private static ServiceLocator? _current;

    public static ServiceLocator Current
    {
        get
        {
            if (_current is not null)
            {
                return _current;
            }
            if (Application.Current!.TryGetResource(nameof(ServiceLocator), out var resource)
                && resource is ServiceLocator serviceLocator)
            {
                return serviceLocator;
            }

            throw new Exception("this should not happen");
        }
    }
    public MainWindowViewModel MainWindowViewModel =>
        _serviceProvider.GetRequiredService<MainWindowViewModel>();

    public InitializationViewModel InitializationViewModel =>
        _serviceProvider.GetRequiredService<InitializationViewModel>();

    public IRootNavigationService RootNavigationService =>
        _serviceProvider.GetRequiredService<IRootNavigationService>();

    public MainViewModel MainViewModel =>
        _serviceProvider.GetRequiredService<MainViewModel>();

    public IWeatherCacheService WeatherCacheService =>
        _serviceProvider.GetRequiredService<IWeatherCacheService>();

    public ISettingsService SettingsService =>
        _serviceProvider.GetRequiredService<ISettingsService>();

    public IContentNavigationService ContentNavigationService =>
        _serviceProvider.GetRequiredService<IContentNavigationService>();

    public WeatherDetailViewModel WeatherDetailViewModel =>
        _serviceProvider.GetRequiredService<WeatherDetailViewModel>();

    public CitiesViewModel CitiesViewModel =>
        _serviceProvider.GetRequiredService<CitiesViewModel>();

    public SettingsViewModel SettingsViewModel =>
        _serviceProvider.GetRequiredService<SettingsViewModel>();

    public AboutViewModel AboutViewModel =>
        _serviceProvider.GetRequiredService<AboutViewModel>();

    public IMenuNavigationService MenuNavigationService =>
        _serviceProvider.GetRequiredService<IMenuNavigationService>();

    public TodayDetailViewModel TodayDetailViewModel =>
        _serviceProvider.GetRequiredService<TodayDetailViewModel>();

    public IBackgroundImageService BackgroundImageService =>
        _serviceProvider.GetRequiredService<IBackgroundImageService>();

    public ServiceLocator()
    {
        var serviceCollection = new ServiceCollection();

        // 添加配置
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        serviceCollection.AddSingleton<IConfiguration>(configuration);

        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<InitializationViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<WeatherDetailViewModel>();
        serviceCollection.AddSingleton<CitiesViewModel>();
        serviceCollection.AddSingleton<SettingsViewModel>();
        serviceCollection.AddSingleton<AboutViewModel>();
        serviceCollection.AddSingleton<TodayDetailViewModel>();

        serviceCollection.AddSingleton<IRootNavigationService, RootNavigationService>();
        serviceCollection.AddSingleton<IWeatherCacheService, WeatherCacheService>();
        serviceCollection.AddSingleton<ISettingsService, SettingsService>();
        serviceCollection.AddSingleton<IContentNavigationService, ContentNavigationService>();
        serviceCollection.AddSingleton<IMenuNavigationService, MenuNavigationService>();
        serviceCollection.AddSingleton<ILocalizationService, LocalizationService>();
        serviceCollection.AddSingleton<IBackgroundImageService, BackgroundImageService>();
        serviceCollection.AddSingleton<IBackgroundImageCacheService, BackgroundImageCacheService>();
        serviceCollection.AddSingleton<IPexelsService, PexelsService>();

        _serviceProvider = serviceCollection.BuildServiceProvider();

    }
}
