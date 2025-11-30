using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WF2.Library.ViewModels;

namespace WF2.Views;

public partial class InitializationView : UserControl
{
    private InitializationViewModel? _viewModel;
    
    public InitializationView()
    {
        InitializeComponent();
        
        // 获取ViewModel
        _viewModel = DataContext as InitializationViewModel;
        
        // 监听主题变化
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(InitializationViewModel.UseDarkTheme))
                {
                    UpdateTheme();
                }
            };
            
            // 初始化主题
            UpdateTheme();
        }
    }
    
    private void UpdateTheme()
    {
        if (_viewModel != null)
        {
            if (_viewModel.UseDarkTheme)
            {
                Classes.Add("dark");
            }
            else
            {
                Classes.Remove("dark");
            }
        }
    }
}