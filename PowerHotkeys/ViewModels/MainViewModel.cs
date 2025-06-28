using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using PowerHotkeysWPF.Models;
using PowerHotkeysWPF.Services;

namespace PowerHotkeysWPF.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ShortcutDataService _dataService;
    private ObservableCollection<Shortcut> _allShortcuts = new();
    private ObservableCollection<Shortcut> _filteredShortcuts = new();
    private string _searchText = string.Empty;
    private string _selectedCategory = "All";
    private ObservableCollection<string> _categories = new();
    private int _gridColumns = 4;

    public MainViewModel()
    {
        _dataService = new ShortcutDataService();
        
        // Initialize commands
        SearchCommand = new RelayCommand(ExecuteSearch);
        RefreshCommand = new RelayCommand(ExecuteRefresh);
        
        // Load data
        LoadDataAsync();
    }

    public ObservableCollection<Shortcut> FilteredShortcuts
    {
        get => _filteredShortcuts;
        set
        {
            _filteredShortcuts = value;
            OnPropertyChanged(nameof(FilteredShortcuts));
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            FilterShortcuts();
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged(nameof(SelectedCategory));
            FilterShortcuts();
        }
    }

    public ObservableCollection<string> Categories
    {
        get => _categories;
        set
        {
            _categories = value;
            OnPropertyChanged(nameof(Categories));
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

    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }

    private async void LoadDataAsync()
    {
        try
        {
            _allShortcuts = await _dataService.LoadShortcutsAsync();
            
            // Update categories
            var categories = new ObservableCollection<string> { "All" };
            var distinctCategories = _allShortcuts.Select(s => s.Category).Distinct().OrderBy(c => c);
            foreach (var category in distinctCategories)
            {
                categories.Add(category);
            }
            Categories = categories;
            
            FilterShortcuts();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load data: {ex.Message}");
        }
    }

    private void FilterShortcuts()
    {
        try
        {
            var filtered = _allShortcuts.AsEnumerable();

            // Filter by category
            if (SelectedCategory != "All")
            {
                filtered = filtered.Where(s => s.Category.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLowerInvariant();
                filtered = filtered.Where(s => 
                    s.Keys.ToLowerInvariant().Contains(searchLower) ||
                    s.Description.ToLowerInvariant().Contains(searchLower) ||
                    s.Application.ToLowerInvariant().Contains(searchLower) ||
                    s.Category.ToLowerInvariant().Contains(searchLower));
            }

            FilteredShortcuts = new ObservableCollection<Shortcut>(filtered);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to filter shortcuts: {ex.Message}");
        }
    }

    private void ExecuteSearch()
    {
        FilterShortcuts();
    }

    private void ExecuteRefresh()
    {
        LoadDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute();
    }
}