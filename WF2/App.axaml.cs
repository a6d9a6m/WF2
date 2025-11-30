using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using WF2.Library.Services;
using WF2.Library.ViewModels;
using WF2.Views;

namespace WF2;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow();

            Console.OutputEncoding = System.Text.Encoding.UTF8;


            ServiceLocator.Current.RootNavigationService.NavigateTo(
                RootNavigationConstant.MainView);
        }

        base.OnFrameworkInitializationCompleted();
    }
    
}