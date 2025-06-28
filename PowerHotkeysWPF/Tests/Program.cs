using PowerHotkeysWPF.Models;
using PowerHotkeysWPF.Services;

namespace PowerHotkeysWPF.Tests;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Testing PowerUser Keys Core Components...");
        
        // Test ShortcutDataService
        var dataService = new ShortcutDataService();
        
        try
        {
            Console.WriteLine("Loading shortcuts...");
            var shortcuts = await dataService.GetShortcutsAsync();
            Console.WriteLine($"Loaded {shortcuts.Count} shortcuts");

            var categories = await dataService.GetCategoriesAsync();
            Console.WriteLine($"Loaded {categories.Count} categories");

            // Test search functionality
            var searchResults = await dataService.SearchShortcutsAsync("copy");
            Console.WriteLine($"Found {searchResults.Count} shortcuts matching 'copy'");

            // Test category filtering
            var windowsShortcuts = await dataService.GetShortcutsByCategoryAsync("Windows System");
            Console.WriteLine($"Found {windowsShortcuts.Count} Windows System shortcuts");

            Console.WriteLine("\nFirst 5 shortcuts:");
            foreach (var shortcut in shortcuts.Take(5))
            {
                Console.WriteLine($"  {shortcut.Keys} - {shortcut.Description} ({shortcut.Application})");
            }

            Console.WriteLine("\nCore components test completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during testing: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}