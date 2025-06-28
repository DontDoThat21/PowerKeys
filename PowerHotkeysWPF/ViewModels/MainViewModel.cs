using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PowerHotkeysWPF.Models;
using PowerHotkeysWPF.Services;

namespace PowerHotkeysWPF.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ShortcutDataService _shortcutDataService;
    private string _searchText = string.Empty;
    private string _selectedCategory = "All";
    private ObservableCollection<Shortcut> _shortcuts = new();
    private ObservableCollection<Shortcut> _filteredShortcuts = new();
    private ObservableCollection<Category> _categories = new();

    public MainViewModel()
    {
        _shortcutDataService = new ShortcutDataService();
        SearchCommand = new RelayCommand(async () => await SearchAsync());
        LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());
        
        // Add "All" category for filtering
        Categories.Add(new Category { Name = "All", Description = "Show all shortcuts" });
        
        // Load data immediately
        _ = LoadDataAsync();
    }

    public ObservableCollection<Shortcut> Shortcuts
    {
        get => _shortcuts;
        set
        {
            _shortcuts = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Shortcut> FilteredShortcuts
    {
        get => _filteredShortcuts;
        set
        {
            _filteredShortcuts = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set
        {
            _categories = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            _ = SearchAsync();
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged();
            _ = FilterByCategoryAsync();
        }
    }

    public ICommand SearchCommand { get; }
    public ICommand LoadDataCommand { get; }

    private async Task LoadDataAsync()
    {
        try
        {
            var shortcuts = await _shortcutDataService.GetShortcutsAsync();
            var categories = await _shortcutDataService.GetCategoriesAsync();

            Shortcuts.Clear();
            Categories.Clear();
            
            // Add "All" category first
            Categories.Add(new Category { Name = "All", Description = "Show all shortcuts" });
            
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            foreach (var shortcut in shortcuts)
            {
                Shortcuts.Add(shortcut);
            }

            await ApplyFiltersAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    private async Task SearchAsync()
    {
        await ApplyFiltersAsync();
    }

    private async Task FilterByCategoryAsync()
    {
        await ApplyFiltersAsync();
    }

    private async Task ApplyFiltersAsync()
    {
        try
        {
            var shortcuts = Shortcuts.ToList();

            // Apply category filter
            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
            {
                shortcuts = shortcuts.Where(s => s.Category.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchTerm = SearchText.ToLowerInvariant();
                shortcuts = shortcuts.Where(s =>
                    s.Description.ToLowerInvariant().Contains(searchTerm) ||
                    s.Keys.ToLowerInvariant().Contains(searchTerm) ||
                    s.Application.ToLowerInvariant().Contains(searchTerm) ||
                    s.Category.ToLowerInvariant().Contains(searchTerm)
                ).ToList();
            }

            FilteredShortcuts.Clear();
            foreach (var shortcut in shortcuts)
            {
                FilteredShortcuts.Add(shortcut);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand : ICommand
{
    private readonly Func<Task> _executeAsync;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
    {
        _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public async void Execute(object? parameter)
    {
        await _executeAsync();
    }
}