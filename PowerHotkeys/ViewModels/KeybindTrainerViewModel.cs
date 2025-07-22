using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using PowerHotkeysWPF.Models;
using PowerHotkeysWPF.Services;

namespace PowerHotkeysWPF.ViewModels;

public class KeybindTrainerViewModel : INotifyPropertyChanged
{
    private readonly ShortcutDataService _dataService;
    private readonly Random _random;
    private ObservableCollection<Shortcut> _allShortcuts = new();
    private List<Shortcut> _shuffledShortcuts = new();
    private int _currentIndex = 0;
    
    private Shortcut? _currentKeybind;
    private string _keybindToGuess = "";
    private int _score = 0;
    private int _totalAttempts = 0;
    private List<string> _currentKeys = new();
    private List<string> _expectedKeys = new();
    private int _currentKeyIndex = 0;
    private bool _isTrainingActive = false;
    private bool _hintUsed = false;

    public KeybindTrainerViewModel()
    {
        _dataService = new ShortcutDataService();
        _random = new Random();
        LoadDataAsync();
    }

    public Shortcut? CurrentKeybind
    {
        get => _currentKeybind;
        set
        {
            _currentKeybind = value;
            OnPropertyChanged(nameof(CurrentKeybind));
        }
    }

    public string KeybindToGuess
    {
        get => _keybindToGuess;
        set
        {
            _keybindToGuess = value;
            OnPropertyChanged(nameof(KeybindToGuess));
        }
    }

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            OnPropertyChanged(nameof(Score));
        }
    }

    public int TotalAttempts
    {
        get => _totalAttempts;
        set
        {
            _totalAttempts = value;
            OnPropertyChanged(nameof(TotalAttempts));
        }
    }

    public bool IsTrainingActive
    {
        get => _isTrainingActive;
        set
        {
            _isTrainingActive = value;
            OnPropertyChanged(nameof(IsTrainingActive));
        }
    }

    public List<string> CurrentKeys => _currentKeys;
    public List<string> ExpectedKeys => _expectedKeys;
    public int CurrentKeyIndex => _currentKeyIndex;

    private async void LoadDataAsync()
    {
        try
        {
            _allShortcuts = await _dataService.LoadShortcutsAsync();
            Debug.WriteLine($"Loaded {_allShortcuts.Count} shortcuts for training");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load shortcuts for training: {ex.Message}");
        }
    }

    public void StartTraining()
    {
        if (_allShortcuts.Count == 0)
        {
            Debug.WriteLine("No shortcuts available for training");
            return;
        }

        // Create a shuffled list of shortcuts for training
        _shuffledShortcuts = _allShortcuts.ToList();
        Shuffle(_shuffledShortcuts);
        _currentIndex = 0;

        // Reset scores
        Score = 0;
        TotalAttempts = 0;
        IsTrainingActive = true;

        // Start with first keybind
        NextKeybind();
    }

    public void NextKeybind()
    {
        if (_shuffledShortcuts.Count == 0) return;

        // Get next keybind (cycle through if needed)
        if (_currentIndex >= _shuffledShortcuts.Count)
        {
            _currentIndex = 0;
            Shuffle(_shuffledShortcuts); // Reshuffle for variety
        }

        CurrentKeybind = _shuffledShortcuts[_currentIndex];
        _currentIndex++;

        // Parse the keybind
        SetupKeybind();
    }

    private void SetupKeybind()
    {
        if (CurrentKeybind == null) return;

        KeybindToGuess = "?";
        _expectedKeys = ParseKeybind(CurrentKeybind.Keys);
        _currentKeys.Clear();
        _currentKeyIndex = 0;
        _hintUsed = false;

        Debug.WriteLine($"Setup keybind: {CurrentKeybind.Keys} -> {string.Join(", ", _expectedKeys)}");
    }

    private List<string> ParseKeybind(string keybind)
    {
        // Parse keybind like "Ctrl + N" or "Alt + Shift + F4"
        return keybind.Split('+')
                     .Select(k => k.Trim())
                     .Where(k => !string.IsNullOrEmpty(k))
                     .ToList();
    }

    public bool ProcessKeyPress(string key)
    {
        if (!IsTrainingActive || _expectedKeys.Count == 0) return false;

        Debug.WriteLine($"Key pressed: {key}, Expected: {(_currentKeyIndex < _expectedKeys.Count ? _expectedKeys[_currentKeyIndex] : "N/A")}");

        // Check if the pressed key matches the expected key
        if (_currentKeyIndex < _expectedKeys.Count && 
            IsKeyMatch(key, _expectedKeys[_currentKeyIndex]))
        {
            _currentKeys.Add(_expectedKeys[_currentKeyIndex]);
            _currentKeyIndex++;

            // Check if all keys have been pressed
            if (_currentKeyIndex >= _expectedKeys.Count)
            {
                // Success!
                TotalAttempts++;
                Score++;
                return true; // Indicates completion
            }
        }
        else
        {
            // Wrong key - reset progress
            _currentKeys.Clear();
            _currentKeyIndex = 0;
        }

        return false; // Not completed yet
    }

    public string UseHint()
    {
        if (!IsTrainingActive || _expectedKeys.Count == 0 || _currentKeyIndex >= _expectedKeys.Count)
            return "";

        _hintUsed = true;
        var hintKey = _expectedKeys[_currentKeyIndex];
        
        // Add the hinted key to progress
        _currentKeys.Add(hintKey);
        _currentKeyIndex++;

        Debug.WriteLine($"Hint used: {hintKey}");
        return hintKey;
    }

    public void FailCurrentAttempt()
    {
        TotalAttempts++;
    }

    private bool IsKeyMatch(string pressedKey, string expectedKey)
    {
        // Normalize key names for comparison
        var pressed = NormalizeKey(pressedKey);
        var expected = NormalizeKey(expectedKey);
        
        return string.Equals(pressed, expected, StringComparison.OrdinalIgnoreCase);
    }

    private string NormalizeKey(string key)
    {
        // Handle common key variations
        return key.Trim().ToLowerInvariant() switch
        {
            "leftctrl" or "rightctrl" or "ctrl" or "control" => "ctrl",
            "leftalt" or "rightalt" or "alt" => "alt",
            "leftshift" or "rightshift" or "shift" => "shift",
            "leftwin" or "rightwin" or "win" or "windows" => "win",
            "space" or "spacebar" => " ",
            "enter" or "return" => "enter",
            "escape" or "esc" => "escape",
            _ => key.Trim().ToLowerInvariant()
        };
    }

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}