using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace PowerHotkeysWPF.Services;

public class HotkeyService : IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint VK_K = 0x4B;
    
    private int _hotkeyId = 1;
    private IntPtr _windowHandle;
    private HwndSource? _source;
    private bool _disposed = false;

    public event EventHandler? HotkeyPressed;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public bool RegisterHotkey(IntPtr windowHandle, string hotkey = "Ctrl+Alt+K")
    {
        try
        {
            _windowHandle = windowHandle;
            _source = HwndSource.FromHwnd(windowHandle);
            
            if (_source != null)
            {
                _source.AddHook(HwndHook);
                
                // Parse hotkey string and get modifiers/key
                var (modifiers, key) = ParseHotkey(hotkey);
                
                return RegisterHotKey(windowHandle, _hotkeyId, modifiers, key);
            }
            
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to register hotkey: {ex.Message}");
            return false;
        }
    }

    public void UnregisterHotkey()
    {
        if (_windowHandle != IntPtr.Zero)
        {
            UnregisterHotKey(_windowHandle, _hotkeyId);
            
            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source = null;
            }
            
            _windowHandle = IntPtr.Zero;
        }
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY && wParam.ToInt32() == _hotkeyId)
        {
            HotkeyPressed?.Invoke(this, EventArgs.Empty);
            handled = true;
        }
        
        return IntPtr.Zero;
    }

    private static (uint modifiers, uint key) ParseHotkey(string hotkey)
    {
        uint modifiers = 0;
        uint key = VK_K; // Default to K
        
        var parts = hotkey.Split('+');
        foreach (var part in parts)
        {
            var trimmed = part.Trim().ToLowerInvariant();
            switch (trimmed)
            {
                case "ctrl":
                case "control":
                    modifiers |= MOD_CONTROL;
                    break;
                case "alt":
                    modifiers |= MOD_ALT;
                    break;
                default:
                    if (trimmed.Length == 1)
                    {
                        key = (uint)char.ToUpperInvariant(trimmed[0]);
                    }
                    break;
            }
        }
        
        return (modifiers, key);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            UnregisterHotkey();
            _disposed = true;
        }
        
        GC.SuppressFinalize(this);
    }
}