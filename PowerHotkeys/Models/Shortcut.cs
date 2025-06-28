using System.ComponentModel;

namespace PowerHotkeysWPF.Models;

public class Shortcut : INotifyPropertyChanged
{
    private string _keys = string.Empty;
    private string _description = string.Empty;
    private string _application = string.Empty;
    private string _category = string.Empty;

    public string Keys
    {
        get => _keys;
        set
        {
            _keys = value;
            OnPropertyChanged(nameof(Keys));
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public string Application
    {
        get => _application;
        set
        {
            _application = value;
            OnPropertyChanged(nameof(Application));
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged(nameof(Category));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}