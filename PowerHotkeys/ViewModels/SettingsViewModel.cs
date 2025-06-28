using System.ComponentModel;
using System.Windows.Input;
using PowerHotkeysWPF.Models;
using PowerHotkeysWPF.Services;

namespace PowerHotkeysWPF.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly ShortcutDataService _dataService;
    private Settings _settings = new();

    public SettingsViewModel()
    {
        _dataService = new ShortcutDataService();
        
        // Initialize commands
        SaveCommand = new RelayCommand(ExecuteSave);
        CancelCommand = new RelayCommand(ExecuteCancel);
        
        // Load settings
        LoadSettingsAsync();
    }

    public Settings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            OnPropertyChanged(nameof(Settings));
            OnPropertyChanged(nameof(GlobalHotkey));
            OnPropertyChanged(nameof(StartWithWindows));
            OnPropertyChanged(nameof(GridColumns));
            OnPropertyChanged(nameof(Theme));
        }
    }

    public string GlobalHotkey
    {
        get => _settings.GlobalHotkey;
        set
        {
            _settings.GlobalHotkey = value;
            OnPropertyChanged(nameof(GlobalHotkey));
        }
    }

    public bool StartWithWindows
    {
        get => _settings.StartWithWindows;
        set
        {
            _settings.StartWithWindows = value;
            OnPropertyChanged(nameof(StartWithWindows));
        }
    }

    public int GridColumns
    {
        get => _settings.GridColumns;
        set
        {
            _settings.GridColumns = value;
            OnPropertyChanged(nameof(GridColumns));
        }
    }

    public string Theme
    {
        get => _settings.Theme;
        set
        {
            _settings.Theme = value;
            OnPropertyChanged(nameof(Theme));
        }
    }

    public string[] AvailableThemes { get; } = { "Light", "Dark", "System" };

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event EventHandler? SettingsSaved;
    public event EventHandler? SettingsCancelled;

    private async void LoadSettingsAsync()
    {
        try
        {
            Settings = await _dataService.LoadSettingsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
        }
    }

    private async void ExecuteSave()
    {
        try
        {
            await _dataService.SaveSettingsAsync(_settings);
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    private void ExecuteCancel()
    {
        LoadSettingsAsync(); // Reload original settings
        SettingsCancelled?.Invoke(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}