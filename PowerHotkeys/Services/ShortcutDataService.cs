using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using PowerHotkeysWPF.Models;
using Shortcut = PowerHotkeysWPF.Models.Shortcut;

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

            // Visual Studio 2022
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + N", Description = "New Project" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + O", Description = "Open Project/Solution" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + O", Description = "Open File" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + S", Description = "Save" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + S", Description = "Save All" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "F5", Description = "Start Debugging" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + F5", Description = "Start Without Debugging" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Shift + F5", Description = "Stop Debugging" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "F9", Description = "Toggle Breakpoint" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "F10", Description = "Step Over" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "F11", Description = "Step Into" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Shift + F11", Description = "Step Out" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + F", Description = "Find" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + H", Description = "Replace" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + F", Description = "Find in Files" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + H", Description = "Replace in Files" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "F12", Description = "Go To Definition" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + F12", Description = "Go To Declaration" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + -", Description = "Navigate Backward" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + -", Description = "Navigate Forward" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + ,", Description = "Go To All" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + T", Description = "Go To File" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + T", Description = "Go To Type" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Alt + A", Description = "Go To Member" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + K, Ctrl + C", Description = "Comment Selection" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + K, Ctrl + U", Description = "Uncomment Selection" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + K, Ctrl + D", Description = "Format Document" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + K, Ctrl + F", Description = "Format Selection" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Space", Description = "IntelliSense" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + Space", Description = "Parameter Info" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + J", Description = "List Members" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + K, Ctrl + I", Description = "Quick Info" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + .", Description = "Quick Actions and Refactorings" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Shift + B", Description = "Build Solution" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Break", Description = "Cancel Build" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Alt + L", Description = "Solution Explorer" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + \\, E", Description = "Error List" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + \\, Ctrl + M", Description = "Team Explorer" },
            new Shortcut { Category = "Visual Studio 2022", Application = "Visual Studio", Keys = "Ctrl + Alt + O", Description = "Output" },

            // ReSharper
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + R", Description = "Refactor This" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + R, R", Description = "Rename" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + R, O", Description = "Move Type to Another File" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + R, V", Description = "Introduce Variable" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + R, M", Description = "Extract Method" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + R, P", Description = "Introduce Parameter" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + R, F", Description = "Introduce Field" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + R, I", Description = "Inline Variable" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + N", Description = "Go to Everything" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + N", Description = "Go to File" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Alt + Shift + N", Description = "Go to Symbol" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + F12", Description = "Go to File Member" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + F12", Description = "Go to Related Files" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "F12", Description = "Go to Declaration" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + F12", Description = "Go to Implementation" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Shift + F12", Description = "Find Usages" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + F", Description = "Find Usages Advanced" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Alt + Enter", Description = "Show Action List" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + E, C", Description = "Recent Edits" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + E, F", Description = "Recent Files" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + Backspace", Description = "Go to Last Edit Location" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Alt + F7", Description = "Find Usages of Symbol at Caret" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Alt + Page Up", Description = "Next Usage" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Alt + Page Down", Description = "Previous Usage" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + Alt + Up", Description = "Go to Next Member" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + Alt + Down", Description = "Go to Previous Member" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + E, H", Description = "View Type Hierarchy" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Alt + F", Description = "Code Formatting" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + E, L", Description = "Code Cleanup" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Alt + Insert", Description = "Generate Code" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Alt + Insert", Description = "Create File from Template" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Ctrl + Shift + A", Description = "Inspect This" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Alt + Page Down", Description = "Next Issue" },
            new Shortcut { Category = "ReSharper", Application = "Visual Studio + ReSharper", Keys = "Alt + Page Up", Description = "Previous Issue" },

            // IntelliJ IDEA
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + N", Description = "Go to Class" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + N", Description = "Go to File" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Alt + Shift + N", Description = "Go to Symbol" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Alt + Right/Left", Description = "Go to Next/Previous Editor Tab" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "F12", Description = "Go Back to Previous Tool Window" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Esc", Description = "Go to Editor" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Shift + Esc", Description = "Hide Active Tool Window" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + F4", Description = "Close Active Editor Tab" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + G", Description = "Go to Line" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + E", Description = "Recent Files Popup" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Alt + Left/Right", Description = "Navigate Back/Forward" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + Backspace", Description = "Navigate to Last Edit Location" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Alt + F1", Description = "Select Current File or Symbol" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + B", Description = "Go to Declaration" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Alt + B", Description = "Go to Implementation(s)" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + I", Description = "Open Quick Definition Lookup" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + B", Description = "Go to Type Declaration" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + U", Description = "Go to Super Method/Super Class" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Alt + Up/Down", Description = "Go to Previous/Next Method" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + ]/[", Description = "Move to Code Block End/Start" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + F12", Description = "File Structure Popup" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + H", Description = "Type Hierarchy" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + H", Description = "Method Hierarchy" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Alt + H", Description = "Call Hierarchy" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "F2", Description = "Next Highlighted Error" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Shift + F2", Description = "Previous Highlighted Error" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "F4", Description = "Edit Source" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Enter", Description = "View Source" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Alt + Home", Description = "Show Navigation Bar" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "F11", Description = "Toggle Bookmark" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + F11", Description = "Toggle Bookmark with Mnemonic" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + 0-9", Description = "Go to Numbered Bookmark" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Shift + F11", Description = "Show Bookmarks" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + F", Description = "Find" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "F3", Description = "Find Next" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Shift + F3", Description = "Find Previous" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + R", Description = "Replace" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + F", Description = "Find in Path" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + R", Description = "Replace in Path" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + S", Description = "Search Structurally" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + M", Description = "Replace Structurally" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Alt + F7", Description = "Find Usages" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + F7", Description = "Find Usages in File" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Shift + F7", Description = "Highlight Usages in File" },
            new Shortcut { Category = "IntelliJ IDEA", Application = "IntelliJ IDEA", Keys = "Ctrl + Alt + F7", Description = "Show Usages" },

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