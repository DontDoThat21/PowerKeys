using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PowerHotkeysWPF.Services;

public class TrayService : IDisposable
{
    private NotifyIcon? _notifyIcon;
    private bool _disposed = false;

    public event EventHandler? ShowRequested;
    public event EventHandler? HideRequested;
    public event EventHandler? ExitRequested;

    public void Initialize()
    {
        try
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = GetApplicationIcon(),
                Text = "PowerKeys - Keyboard Shortcuts",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (s, e) => ShowRequested?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add("Hide", null, (s, e) => HideRequested?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("Exit", null, (s, e) => ExitRequested?.Invoke(this, EventArgs.Empty));

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => ShowRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to initialize tray: {ex.Message}");
        }
    }

    private static Icon GetApplicationIcon()
    {
        try
        {
            // Try to load custom icon
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon.ico");
            if (File.Exists(iconPath))
            {
                return new Icon(iconPath);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load custom icon: {ex.Message}");
        }

        // Fallback to default system icon
        return SystemIcons.Application;
    }

    public void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
    {
        try
        {
            _notifyIcon?.ShowBalloonTip(3000, title, text, icon);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to show balloon tip: {ex.Message}");
        }
    }

    public void Hide()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
        }
    }

    public void Show()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = true;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _notifyIcon?.Dispose();
            _disposed = true;
        }
        
        GC.SuppressFinalize(this);
    }
}