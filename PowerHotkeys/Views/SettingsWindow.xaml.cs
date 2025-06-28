using System.Windows;
using PowerHotkeysWPF.ViewModels;

namespace PowerHotkeysWPF.Views;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly SettingsViewModel _viewModel;

    public SettingsWindow()
    {
        InitializeComponent();
        
        _viewModel = new SettingsViewModel();
        DataContext = _viewModel;
        
        _viewModel.SettingsSaved += OnSettingsSaved;
        _viewModel.SettingsCancelled += OnSettingsCancelled;
    }

    private void OnSettingsSaved(object? sender, EventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnSettingsCancelled(object? sender, EventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SaveCommand.Execute(null);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.CancelCommand.Execute(null);
    }
}