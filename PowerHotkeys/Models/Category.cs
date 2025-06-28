using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PowerHotkeysWPF.Models;

public class Category : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _color = "#007ACC";
    private ObservableCollection<Shortcut> _shortcuts = new();

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string Color
    {
        get => _color;
        set
        {
            _color = value;
            OnPropertyChanged(nameof(Color));
        }
    }

    public ObservableCollection<Shortcut> Shortcuts
    {
        get => _shortcuts;
        set
        {
            _shortcuts = value;
            OnPropertyChanged(nameof(Shortcuts));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}