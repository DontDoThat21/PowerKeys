using Newtonsoft.Json;
using PowerHotkeysWPF.Models;
using System.IO;

namespace PowerHotkeysWPF.Services;

public class ShortcutDataService
{
    private readonly string _dataFilePath;
    private List<Shortcut> _shortcuts = new();
    private List<Category> _categories = new();

    public ShortcutDataService()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PowerHotkeysWPF");
        Directory.CreateDirectory(appDataPath);
        _dataFilePath = Path.Combine(appDataPath, "shortcuts.json");
    }

    public async Task<List<Shortcut>> GetShortcutsAsync()
    {
        if (_shortcuts.Count == 0)
        {
            await LoadDataAsync();
        }
        return _shortcuts.ToList();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        if (_categories.Count == 0)
        {
            await LoadDataAsync();
        }
        return _categories.ToList();
    }

    public async Task<List<Shortcut>> GetShortcutsByCategoryAsync(string categoryName)
    {
        var shortcuts = await GetShortcutsAsync();
        return shortcuts.Where(s => s.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<Shortcut>> SearchShortcutsAsync(string searchTerm)
    {
        var shortcuts = await GetShortcutsAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return shortcuts;

        var term = searchTerm.ToLowerInvariant();
        return shortcuts.Where(s => 
            s.Description.ToLowerInvariant().Contains(term) ||
            s.Keys.ToLowerInvariant().Contains(term) ||
            s.Application.ToLowerInvariant().Contains(term) ||
            s.Category.ToLowerInvariant().Contains(term)
        ).ToList();
    }

    public async Task AddShortcutAsync(Shortcut shortcut)
    {
        var shortcuts = await GetShortcutsAsync();
        shortcut.Id = Guid.NewGuid().ToString();
        shortcut.CreatedDate = DateTime.Now;
        shortcut.LastModified = DateTime.Now;
        shortcuts.Add(shortcut);
        _shortcuts = shortcuts;
        await SaveDataAsync();
    }

    public async Task UpdateShortcutAsync(Shortcut shortcut)
    {
        var shortcuts = await GetShortcutsAsync();
        var existing = shortcuts.FirstOrDefault(s => s.Id == shortcut.Id);
        if (existing != null)
        {
            existing.Application = shortcut.Application;
            existing.Category = shortcut.Category;
            existing.Keys = shortcut.Keys;
            existing.Description = shortcut.Description;
            existing.IsVisible = shortcut.IsVisible;
            existing.LastModified = DateTime.Now;
            await SaveDataAsync();
        }
    }

    public async Task DeleteShortcutAsync(string shortcutId)
    {
        var shortcuts = await GetShortcutsAsync();
        _shortcuts = shortcuts.Where(s => s.Id != shortcutId).ToList();
        await SaveDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            string jsonContent;
            
            // If user data file doesn't exist, load from embedded resource
            if (!File.Exists(_dataFilePath))
            {
                var resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shortcuts.json");
                
                if (File.Exists(resourcePath))
                {
                    jsonContent = await File.ReadAllTextAsync(resourcePath);
                }
                else
                {
                    // Try alternative path for WPF applications
                    var altResourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "shortcuts.json");
                    if (File.Exists(altResourcePath))
                    {
                        jsonContent = await File.ReadAllTextAsync(altResourcePath);
                    }
                    else
                    {
                        // Fallback to empty data
                        _shortcuts = new List<Shortcut>();
                        _categories = new List<Category>();
                        return;
                    }
                }
            }
            else
            {
                jsonContent = await File.ReadAllTextAsync(_dataFilePath);
            }

            var data = JsonConvert.DeserializeAnonymousType(jsonContent, new
            {
                categories = new List<Category>(),
                shortcuts = new List<Shortcut>()
            });

            _categories = data?.categories ?? new List<Category>();
            _shortcuts = data?.shortcuts ?? new List<Shortcut>();

            // Generate IDs for shortcuts that don't have them
            foreach (var shortcut in _shortcuts.Where(s => string.IsNullOrEmpty(s.Id)))
            {
                shortcut.Id = Guid.NewGuid().ToString();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading shortcuts data: {ex.Message}");
            _shortcuts = new List<Shortcut>();
            _categories = new List<Category>();
        }
    }

    private async Task SaveDataAsync()
    {
        try
        {
            var data = new
            {
                categories = _categories,
                shortcuts = _shortcuts
            };

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving shortcuts data: {ex.Message}");
        }
    }
}