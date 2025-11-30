using Avalonia.Controls;
using WF2.Library.ViewModels;

namespace WF2.Views;

public partial class SettingsView : UserControl
{
    private SettingsViewModel _viewModel;

    public SettingsView()
    {
        InitializeComponent();
        
        _viewModel = DataContext as SettingsViewModel;
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.UseDarkTheme))
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
