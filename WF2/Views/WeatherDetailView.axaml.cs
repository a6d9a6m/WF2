using Avalonia.Controls;
using WF2.Library.ViewModels;

namespace WF2.Views;

public partial class WeatherDetailView : UserControl
{
    public WeatherDetailView()
    {
        InitializeComponent();
        
        // 监听主题变化
        if (DataContext is WeatherDetailViewModel viewModel)
        {
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(WeatherDetailViewModel.UseDarkTheme))
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
