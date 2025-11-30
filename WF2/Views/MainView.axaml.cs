
using Avalonia.Controls;
using Avalonia.Data.Core;
using WF2.Library.ViewModels;

namespace WF2.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        
        // 监听主题变化
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.UseDarkTheme))
                {
                    UpdateTheme(viewModel.UseDarkTheme);
                }
            };
            
            // 初始化主题
            UpdateTheme(viewModel.UseDarkTheme);
        }
    }
    
    private void UpdateTheme(bool isDark)
    {
        if (isDark)
        {
            Classes.Add("dark");
        }
        else
        {
            Classes.Remove("dark");
        }
    }
}