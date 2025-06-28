using System.Windows;

namespace PowerHotkeysWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Ensure only one instance is running
        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
        var runningProcesses = System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName);
        
        if (runningProcesses.Length > 1)
        {
            // Another instance is already running
            Current.Shutdown();
            return;
        }

        base.OnStartup(e);
        
        // Create and show main window (it will minimize to tray automatically)
        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}

