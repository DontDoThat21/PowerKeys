using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using PowerHotkeysWPF.Services;
using PowerHotkeysWPF.ViewModels;
using PowerHotkeysWPF.Views;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace PowerHotkeysWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly HotkeyService _hotkeyService;
    private readonly TrayService _trayService;
    private readonly ShortcutDataService _dataService;
    private readonly MainViewModel _viewModel;
    private bool _isClosing = false;

    public MainWindow()
    {
        InitializeComponent();
        
        // Initialize services
        _hotkeyService = new HotkeyService();
        _trayService = new TrayService();
        _dataService = new ShortcutDataService();
        
        // Initialize ViewModel
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        
        // Initialize components
        InitializeWindow();
        InitializeServices();
        
        // Load settings
        LoadSettingsAsync();
    }

    private void InitializeWindow()
    {
        // Window properties
        ShowInTaskbar = false;
        WindowStyle = WindowStyle.SingleBorderWindow;
        Topmost = true;
        
        // Hide window initially
        Hide();
        
        // Window state management
        StateChanged += MainWindow_StateChanged;
        Closing += MainWindow_Closing;
        Deactivated += MainWindow_Deactivated;
    }

    private void InitializeServices()
    {
        // Initialize tray service
        _trayService.Initialize();
        _trayService.ShowRequested += (s, e) => ShowWindow();
        _trayService.HideRequested += (s, e) => HideWindow();
        _trayService.ExitRequested += (s, e) => ExitApplication();
        
        // Initialize hotkey service (will be done after window is loaded)
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var windowHandle = new WindowInteropHelper(this).Handle;
            if (_hotkeyService.RegisterHotkey(windowHandle))
            {
                _hotkeyService.HotkeyPressed += (s, e) => ToggleWindow();
            }
            else
            {
                _trayService.ShowBalloonTip("PowerKeys", 
                    "Failed to register global hotkey Ctrl+Alt+K. Another application may be using it.",
                    System.Windows.Forms.ToolTipIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to register hotkey: {ex.Message}");
        }
    }

    private async void LoadSettingsAsync()
    {
        try
        {
            var settings = await _dataService.LoadSettingsAsync();
            _viewModel.GridColumns = settings.GridColumns;
            
            // Restore window position and size
            if (!double.IsNaN(settings.WindowWidth) && settings.WindowWidth > 0)
                Width = settings.WindowWidth;
            if (!double.IsNaN(settings.WindowHeight) && settings.WindowHeight > 0)
                Height = settings.WindowHeight;
            if (!double.IsNaN(settings.WindowLeft) && !double.IsNaN(settings.WindowTop))
            {
                Left = settings.WindowLeft;
                Top = settings.WindowTop;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
        }
    }

    private async void SaveSettingsAsync()
    {
        try
        {
            var settings = await _dataService.LoadSettingsAsync();
            settings.GridColumns = _viewModel.GridColumns;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            
            await _dataService.SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    private void ShowWindow()
    {
        try
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
            Focus();
            
            // Fade in animation
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            BeginAnimation(OpacityProperty, fadeIn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to show window: {ex.Message}");
        }
    }

    private void HideWindow()
    {
        try
        {
            // Fade out animation
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, e) => Hide();
            BeginAnimation(OpacityProperty, fadeOut);
            
            SaveSettingsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to hide window: {ex.Message}");
            Hide();
        }
    }

    private void ToggleWindow()
    {
        try
        {
            if (IsVisible && WindowState != WindowState.Minimized)
            {
                HideWindow();
            }
            else
            {
                ShowWindow();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to toggle window: {ex.Message}");
        }
    }

    private void ExitApplication()
    {
        _isClosing = true;
        SaveSettingsAsync();
        Application.Current.Shutdown();
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            HideWindow();
        }
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        if (!_isClosing)
        {
            e.Cancel = true;
            HideWindow();
        }
        else
        {
            // Cleanup
            _hotkeyService?.Dispose();
            _trayService?.Dispose();
        }
    }

    private void MainWindow_Deactivated(object? sender, EventArgs e)
    {
        // Optional: Hide when clicked outside
        // HideWindow();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to show settings: {ex.Message}");
            MessageBox.Show("Failed to open settings window.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        
        // Additional window setup can go here
    }
}