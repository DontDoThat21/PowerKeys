using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace PowerHotkeysWPF.Services;

public class TrayService
{
    private TaskbarIcon? _notifyIcon;
    private Window? _mainWindow;

    public event EventHandler? ShowRequested;
    public event EventHandler? HideRequested;
    public event EventHandler? ExitRequested;

    public void Initialize(Window mainWindow)
    {
        _mainWindow = mainWindow;
        
        _notifyIcon = new TaskbarIcon
        {
            IconSource = new System.Windows.Media.Imaging.BitmapImage(
                new Uri("pack://application:,,,/Resources/icon.ico", UriKind.RelativeOrAbsolute)),
            ToolTipText = "PowerUser Keys - Ctrl+Alt+K to show/hide"
        };

        // Handle double-click to show/hide
        _notifyIcon.TrayMouseDoubleClick += (s, e) =>
        {
            if (_mainWindow?.IsVisible == true)
            {
                HideRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ShowRequested?.Invoke(this, EventArgs.Empty);
            }
        };

        // Create context menu
        var contextMenu = new System.Windows.Controls.ContextMenu();
        
        var showHideMenuItem = new System.Windows.Controls.MenuItem
        {
            Header = "Show/Hide"
        };
        showHideMenuItem.Click += (s, e) =>
        {
            if (_mainWindow?.IsVisible == true)
            {
                HideRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ShowRequested?.Invoke(this, EventArgs.Empty);
            }
        };
        
        var separatorMenuItem = new System.Windows.Controls.Separator();
        
        var exitMenuItem = new System.Windows.Controls.MenuItem
        {
            Header = "Exit"
        };
        exitMenuItem.Click += (s, e) =>
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        };

        contextMenu.Items.Add(showHideMenuItem);
        contextMenu.Items.Add(separatorMenuItem);
        contextMenu.Items.Add(exitMenuItem);

        _notifyIcon.ContextMenu = contextMenu;
    }

    public void ShowBalloonTip(string title, string message, BalloonIcon icon = BalloonIcon.Info)
    {
        _notifyIcon?.ShowBalloonTip(title, message, icon);
    }

    public void Dispose()
    {
        _notifyIcon?.Dispose();
    }
}