using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using PowerHotkeysWPF.Models;

namespace PowerHotkeysWPF.Services;

public class ShortcutDataService
{
    private readonly string _dataPath;
    private readonly string _settingsPath;

    public ShortcutDataService()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PowerKeys");
        Directory.CreateDirectory(appDataPath);
        
        _dataPath = Path.Combine(appDataPath, "shortcuts.json");
        _settingsPath = Path.Combine(appDataPath, "settings.json");
    }

    public async Task<ObservableCollection<Shortcut>> LoadShortcutsAsync()
    {
        try
        {
            string jsonContent;
            
            // Try to load from user data first
            if (File.Exists(_dataPath))
            {
                jsonContent = await File.ReadAllTextAsync(_dataPath);
            }
            else
            {
                // Load from embedded resource
                var resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "shortcuts.json");
                if (File.Exists(resourcePath))
                {
                    jsonContent = await File.ReadAllTextAsync(resourcePath);
                }
                else
                {
                    // Return default shortcuts if no file exists
                    return GetDefaultShortcuts();
                }
            }

            var shortcuts = JsonConvert.DeserializeObject<ObservableCollection<Shortcut>>(jsonContent);
            return shortcuts ?? GetDefaultShortcuts();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load shortcuts: {ex.Message}");
            return GetDefaultShortcuts();
        }
    }

    public async Task SaveShortcutsAsync(ObservableCollection<Shortcut> shortcuts)
    {
        try
        {
            var json = JsonConvert.SerializeObject(shortcuts, Formatting.Indented);
            await File.WriteAllTextAsync(_dataPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save shortcuts: {ex.Message}");
        }
    }

    public async Task<Settings> LoadSettingsAsync()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = await File.ReadAllTextAsync(_settingsPath);
                var settings = JsonConvert.DeserializeObject<Settings>(json);
                return settings ?? new Settings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
        }
        
        return new Settings();
    }

    public async Task SaveSettingsAsync(Settings settings)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(_settingsPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    private static ObservableCollection<Shortcut> GetDefaultShortcuts()
    {
        return new ObservableCollection<Shortcut>
        {
            // Windows System
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + L", Description = "Lock Screen" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + D", Description = "Show Desktop" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Alt + Tab", Description = "Switch Apps" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + Tab", Description = "Task View" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + I", Description = "Settings" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + R", Description = "Run Dialog" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + X", Description = "Quick Menu" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + E", Description = "File Explorer" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + A", Description = "Action Center" },
            new Shortcut { Category = "Windows System", Application = "Windows", Keys = "Win + S", Description = "Search" },

            // Text Editing
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Ctrl + Shift + K", Description = "Delete Line" },
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Ctrl + D", Description = "Duplicate Line" },
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Alt + Up/Down", Description = "Move Line" },
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Ctrl + L", Description = "Select Line" },
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Ctrl + Shift + L", Description = "Select All Occurrences" },
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Ctrl + F2", Description = "Select All Occurrences" },
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Ctrl + /", Description = "Toggle Comment" },
            new Shortcut { Category = "Text Editing", Application = "Universal", Keys = "Ctrl + Shift + /", Description = "Block Comment" },

            // Browser
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + T", Description = "New Tab" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + W", Description = "Close Tab" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + Shift + T", Description = "Reopen Tab" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + Tab", Description = "Next Tab" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + Shift + Tab", Description = "Previous Tab" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + L", Description = "Address Bar" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + R", Description = "Refresh" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "Ctrl + Shift + R", Description = "Hard Refresh" },
            new Shortcut { Category = "Browser", Application = "Chrome/Edge", Keys = "F12", Description = "Developer Tools" },

            // VS Code
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + Shift + P", Description = "Command Palette" },
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + P", Description = "Quick Open" },
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + Shift + E", Description = "Explorer" },
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + Shift + F", Description = "Search" },
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + Shift + G", Description = "Source Control" },
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + Shift + D", Description = "Debug" },
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + Shift + X", Description = "Extensions" },
            new Shortcut { Category = "VS Code", Application = "Visual Studio Code", Keys = "Ctrl + `", Description = "Terminal" },

            // Office
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + N", Description = "New Document" },
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + O", Description = "Open" },
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + S", Description = "Save" },
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + Shift + S", Description = "Save As" },
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + P", Description = "Print" },
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + B", Description = "Bold" },
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + I", Description = "Italic" },
            new Shortcut { Category = "Office", Application = "Word/Excel", Keys = "Ctrl + U", Description = "Underline" }
        };
    }
}