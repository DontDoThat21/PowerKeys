using System.Windows;
using System.Windows.Media.Animation;
using PowerHotkeysWPF.Services;
using PowerHotkeysWPF.ViewModels;

namespace PowerHotkeysWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly HotkeyService _hotkeyService;
    private readonly TrayService _trayService;
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        
        _hotkeyService = new HotkeyService();
        _trayService = new TrayService();
        
        InitializeServices();
        InitializeAnimations();
        
        // Hide window on startup
        WindowState = WindowState.Minimized;
        Hide();
    }

    private void InitializeServices()
    {
        // Setup hotkey service
        _hotkeyService.HotkeyPressed += OnHotkeyPressed;
        
        // Setup tray service
        _trayService.Initialize(this);
        _trayService.ShowRequested += OnShowRequested;
        _trayService.HideRequested += OnHideRequested;
        _trayService.ExitRequested += OnExitRequested;
        
        // Register hotkey when window is loaded
        Loaded += (s, e) =>
        {
            if (!_hotkeyService.RegisterHotkey(this))
            {
                _trayService.ShowBalloonTip("PowerUser Keys", 
                    "Warning: Could not register global hotkey Ctrl+Alt+K. It may be in use by another application.", 
                    Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }
        };
    }

    private void InitializeAnimations()
    {
        // Set initial opacity to 0 for fade-in animation
        Opacity = 0;
    }

    private void OnHotkeyPressed(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            if (IsVisible && WindowState != WindowState.Minimized)
            {
                HideWindow();
            }
            else
            {
                ShowWindow();
            }
        });
    }

    private void OnShowRequested(object? sender, EventArgs e)
    {
        ShowWindow();
    }

    private void OnHideRequested(object? sender, EventArgs e)
    {
        HideWindow();
    }

    private void OnExitRequested(object? sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ShowWindow()
    {
        try
        {
            // Restore window state
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }

            // Show window
            Show();
            Activate();
            Focus();

            // Fade in animation
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            BeginAnimation(OpacityProperty, fadeIn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing window: {ex.Message}");
        }
    }

    private void HideWindow()
    {
        try
        {
            // Fade out animation
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOut.Completed += (s, e) =>
            {
                Hide();
                Opacity = 0; // Reset opacity for next show
            };

            BeginAnimation(OpacityProperty, fadeOut);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error hiding window: {ex.Message}");
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        HideWindow();
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
        }
        base.OnStateChanged(e);
    }

    protected override void OnDeactivated(EventArgs e)
    {
        // Optionally hide when window loses focus
        // Uncomment the next line if you want click-outside-to-hide behavior
        // HideWindow();
        base.OnDeactivated(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        _hotkeyService?.Dispose();
        _trayService?.Dispose();
        base.OnClosed(e);
    }
}