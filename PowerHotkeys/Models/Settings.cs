using System.ComponentModel;

namespace PowerHotkeysWPF.Models;

public class Settings : INotifyPropertyChanged
{
    private string _globalHotkey = "Ctrl+Alt+K";
    private bool _startWithWindows = true;
    private int _gridColumns = 4;
    private string _theme = "System";
    private double _windowWidth = 800;
    private double _windowHeight = 600;
    private double _windowLeft = double.NaN;
    private double _windowTop = double.NaN;

    public string GlobalHotkey
    {
        get => _globalHotkey;
        set
        {
            _globalHotkey = value;
            OnPropertyChanged(nameof(GlobalHotkey));
        }
    }

    public bool StartWithWindows
    {
        get => _startWithWindows;
        set
        {
            _startWithWindows = value;
            OnPropertyChanged(nameof(StartWithWindows));
        }
    }

    public int GridColumns
    {
        get => _gridColumns;
        set
        {
            _gridColumns = Math.Max(3, Math.Min(8, value));
            OnPropertyChanged(nameof(GridColumns));
        }
    }

    public string Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            OnPropertyChanged(nameof(Theme));
        }
    }

    public double WindowWidth
    {
        get => _windowWidth;
        set
        {
            _windowWidth = value;
            OnPropertyChanged(nameof(WindowWidth));
        }
    }

    public double WindowHeight
    {
        get => _windowHeight;
        set
        {
            _windowHeight = value;
            OnPropertyChanged(nameof(WindowHeight));
        }
    }

    public double WindowLeft
    {
        get => _windowLeft;
        set
        {
            _windowLeft = value;
            OnPropertyChanged(nameof(WindowLeft));
        }
    }

    public double WindowTop
    {
        get => _windowTop;
        set
        {
            _windowTop = value;
            OnPropertyChanged(nameof(WindowTop));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}