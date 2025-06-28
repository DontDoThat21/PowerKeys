using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PowerHotkeysWPF.Services;

public class HotkeyService
{
    private const int WM_HOTKEY = 0x0312;
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;
    
    private const uint VK_K = 0x4B;
    
    private readonly int _hotkeyId = 9000;
    private IntPtr _windowHandle;
    private HwndSource? _source;
    private bool _isRegistered = false;

    public event EventHandler? HotkeyPressed;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public bool RegisterHotkey(Window window, uint modifiers = MOD_CONTROL | MOD_ALT, uint key = VK_K)
    {
        try
        {
            var helper = new WindowInteropHelper(window);
            _windowHandle = helper.Handle;

            if (_windowHandle == IntPtr.Zero)
            {
                window.SourceInitialized += (s, e) =>
                {
                    var newHelper = new WindowInteropHelper(window);
                    _windowHandle = newHelper.Handle;
                    RegisterHotkeyInternal(modifiers, key);
                };
            }
            else
            {
                return RegisterHotkeyInternal(modifiers, key);
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error registering hotkey: {ex.Message}");
            return false;
        }
    }

    private bool RegisterHotkeyInternal(uint modifiers, uint key)
    {
        try
        {
            if (_isRegistered)
            {
                UnregisterHotKey(_windowHandle, _hotkeyId);
                _isRegistered = false;
            }

            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);

            _isRegistered = RegisterHotKey(_windowHandle, _hotkeyId, modifiers, key);
            
            if (!_isRegistered)
            {
                System.Diagnostics.Debug.WriteLine("Failed to register hotkey. It might be already in use by another application.");
            }

            return _isRegistered;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in RegisterHotkeyInternal: {ex.Message}");
            return false;
        }
    }

    public void UnregisterHotkey()
    {
        try
        {
            if (_isRegistered && _windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, _hotkeyId);
                _isRegistered = false;
            }

            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source = null;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error unregistering hotkey: {ex.Message}");
        }
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY)
        {
            var id = wParam.ToInt32();
            if (id == _hotkeyId)
            {
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
                handled = true;
            }
        }
        return IntPtr.Zero;
    }

    public void Dispose()
    {
        UnregisterHotkey();
    }
}