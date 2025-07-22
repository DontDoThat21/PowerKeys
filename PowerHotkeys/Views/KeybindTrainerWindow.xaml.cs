using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using PowerHotkeysWPF.ViewModels;
using Color = System.Windows.Media.Color;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Point = System.Windows.Point;

namespace PowerHotkeysWPF.Views;

public partial class KeybindTrainerWindow : Window
{
    private readonly KeybindTrainerViewModel _viewModel;
    private readonly Dictionary<Key, string> _keyMappings;

    public KeybindTrainerWindow()
    {
        InitializeComponent();
        
        _viewModel = new KeybindTrainerViewModel();
        DataContext = _viewModel;
        
        // Initialize key mappings for key press detection
        _keyMappings = InitializeKeyMappings();
        
        // Set focus to window to capture key events
        Focusable = true;
        KeyDown += KeybindTrainerWindow_KeyDown;
        Loaded += KeybindTrainerWindow_Loaded;
    }

    private void KeybindTrainerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Focus();
        UpdateProgressDisplay();
    }

    private Dictionary<Key, string> InitializeKeyMappings()
    {
        return new Dictionary<Key, string>
        {
            { Key.LeftCtrl, "Ctrl" },
            { Key.RightCtrl, "Ctrl" },
            { Key.LeftAlt, "Alt" },
            { Key.RightAlt, "Alt" },
            { Key.LeftShift, "Shift" },
            { Key.RightShift, "Shift" },
            { Key.LWin, "Win" },
            { Key.RWin, "Win" },
            { Key.Space, "Space" },
            { Key.Enter, "Enter" },
            { Key.Escape, "Escape" },
            { Key.Tab, "Tab" },
            { Key.Back, "Backspace" },
            { Key.Delete, "Delete" },
            { Key.Home, "Home" },
            { Key.End, "End" },
            { Key.PageUp, "PageUp" },
            { Key.PageDown, "PageDown" },
            { Key.Up, "Up" },
            { Key.Down, "Down" },
            { Key.Left, "Left" },
            { Key.Right, "Right" },
            { Key.F1, "F1" }, { Key.F2, "F2" }, { Key.F3, "F3" }, { Key.F4, "F4" },
            { Key.F5, "F5" }, { Key.F6, "F6" }, { Key.F7, "F7" }, { Key.F8, "F8" },
            { Key.F9, "F9" }, { Key.F10, "F10" }, { Key.F11, "F11" }, { Key.F12, "F12" }
        };
    }

    private void KeybindTrainerWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (!_viewModel.IsTrainingActive) return;

        string keyPressed = GetKeyString(e.Key);
        Debug.WriteLine($"Key captured: {e.Key} -> {keyPressed}");

        // Special handling for spacebar (hint)
        if (e.Key == Key.Space)
        {
            ShowHint();
            e.Handled = true;
            return;
        }

        // Process the key press
        bool completed = _viewModel.ProcessKeyPress(keyPressed);
        
        UpdateProgressDisplay();
        
        if (completed)
        {
            ShowSuccess();
        }
        
        e.Handled = true;
    }

    private string GetKeyString(Key key)
    {
        if (_keyMappings.TryGetValue(key, out string mapped))
            return mapped;

        // Handle regular letter/number keys
        string keyStr = key.ToString();
        
        // Handle number keys (D0-D9)
        if (keyStr.StartsWith("D") && keyStr.Length == 2 && char.IsDigit(keyStr[1]))
            return keyStr[1].ToString();
        
        // Handle NumPad numbers
        if (keyStr.StartsWith("NumPad"))
            return keyStr.Replace("NumPad", "");

        return keyStr;
    }

    private void ShowHint()
    {
        string hintKey = _viewModel.UseHint();
        if (!string.IsNullOrEmpty(hintKey))
        {
            InstructionText.Text = $"Hint: Press '{hintKey}' next";
            UpdateProgressDisplay();
            
            // Reset instruction after a delay
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) =>
            {
                InstructionText.Text = "Press the keys to match the keybind above.\nPress Spacebar for a hint if you're stuck.";
                timer.Stop();
            };
            timer.Start();
        }
    }

    private void ShowSuccess()
    {
        // Update UI to show success
        FeedbackText.Text = "Correct! ✓";
        FeedbackText.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127)); // Neon green
        
        // Add glow effect
        var dropShadow = new DropShadowEffect
        {
            Color = Color.FromRgb(0, 255, 127),
            BlurRadius = 15,
            ShadowDepth = 0
        };
        FeedbackText.Effect = dropShadow;

        // Animate the feedback
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        var scaleTransform = new ScaleTransform(1.0, 1.0);
        FeedbackText.RenderTransform = scaleTransform;
        FeedbackText.RenderTransformOrigin = new Point(0.5, 0.5);
        
        var scaleAnimation = new DoubleAnimation(1.0, 1.2, TimeSpan.FromMilliseconds(200))
        {
            AutoReverse = true
        };

        FeedbackText.BeginAnimation(OpacityProperty, fadeIn);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

        // Enable Next button
        NextButton.IsEnabled = true;
        
        // Auto-advance after delay
        var timer = new System.Windows.Threading.DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(2);
        timer.Tick += (s, e) =>
        {
            NextButton_Click(NextButton, new RoutedEventArgs());
            timer.Stop();
        };
        timer.Start();
    }

    private void UpdateProgressDisplay()
    {
        ProgressPanel.Children.Clear();
        
        if (_viewModel.ExpectedKeys.Count == 0) return;

        for (int i = 0; i < _viewModel.ExpectedKeys.Count; i++)
        {
            var keyBlock = new TextBlock
            {
                Text = _viewModel.ExpectedKeys[i],
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Padding = new System.Windows.Thickness(8, 4, 8, 4),
                Margin = new System.Windows.Thickness(2, 2, 2, 2)
            };
            keyBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            keyBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            var border = new Border
            {
                Child = keyBlock,
                BorderThickness = new System.Windows.Thickness(2, 2, 2, 2),
                CornerRadius = new CornerRadius(4),
                Padding = new System.Windows.Thickness(4, 4, 4, 4)
            };

            if (i < _viewModel.CurrentKeyIndex)
            {
                // Already pressed - green
                keyBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                border.Background = new SolidColorBrush(Color.FromArgb(30, 0, 255, 127));
            }
            else if (i == _viewModel.CurrentKeyIndex)
            {
                // Current key to press - highlight
                keyBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 208, 132));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 208, 132));
                border.Background = new SolidColorBrush(Color.FromArgb(50, 0, 208, 132));
            }
            else
            {
                // Not yet pressed - dim
                keyBlock.Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                border.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            }

            ProgressPanel.Children.Add(border);

            // Add separator if not last key
            if (i < _viewModel.ExpectedKeys.Count - 1)
            {
                var separator = new TextBlock
                {
                    Text = "+",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Color.FromRgb(176, 176, 176)),
                    Margin = new System.Windows.Thickness(4, 4, 4, 4)
                };
                separator.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                ProgressPanel.Children.Add(separator);
            }
        }
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.StartTraining();
        StartButton.IsEnabled = false;
        NextButton.IsEnabled = false;
        FeedbackText.Text = "";
        FeedbackText.Effect = null;
        Focus(); // Ensure window has focus for key events
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.NextKeybind();
        NextButton.IsEnabled = false;
        FeedbackText.Text = "";
        FeedbackText.Effect = null;
        UpdateProgressDisplay();
        Focus(); // Ensure window has focus for key events
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}