using Avalonia.Controls;
using WF2.Library.ViewModels;

namespace WF2.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        
        // 获取ViewModel并监听主题变化
        if (DataContext is MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.UseDarkTheme))
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

