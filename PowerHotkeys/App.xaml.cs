using System.Windows;

namespace PowerHotkeysWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Ensure only one instance of the application runs
        if (IsAlreadyRunning())
        {
            MessageBox.Show("PowerKeys is already running. Check the system tray.", 
                          "PowerKeys", MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }

        // Create and configure main window
        _mainWindow = new MainWindow();
        MainWindow = _mainWindow;

        // Don't show the window immediately - it will be controlled by tray/hotkey
        // _mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }

    private static bool IsAlreadyRunning()
    {
        try
        {
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var processes = System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName);
            
            return processes.Length > 1;
        }
        catch
        {
            return false;
        }
    }
}

