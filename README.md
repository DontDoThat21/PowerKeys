# PowerKeys - Keyboard Shortcuts Reference

A Windows desktop application that displays a grid of useful keyboard shortcuts for power users. The app runs silently in the background, accessible via system tray, and can be instantly shown/hidden using a global hotkey combination.

## Features

### 🚀 Core Features (MVP)

- **System Tray Integration**
  - Minimizes to system tray on startup
  - Right-click context menu with Show/Hide/Exit options
  - Double-click tray icon to show/hide window

- **Global Hotkey Support**
  - Default hotkey: `Ctrl + Alt + K` to show/hide the window
  - Works regardless of which application has focus
  - Graceful handling of hotkey conflicts

- **Responsive Grid Display**
  - Shows 40+ pre-defined shortcuts in organized grid layout
  - 4-6 columns that auto-adjust based on window size
  - Color-coded categories (Windows, Browser, VS Code, Office, etc.)
  - Real-time search and filter functionality

- **Window Management**
  - Window appears centered on screen
  - Always stays on top when visible
  - Smooth fade in/out animations (200ms duration)
  - Remembers last window position and size

### ⚙️ Settings & Customization

- **Global Hotkey Customization** - Change the hotkey combination
- **Startup with Windows** - Toggle auto-start behavior
- **Grid Layout Options** - Adjust number of columns (3-8)
- **Theme Selection** - Light/Dark/System themes
- **Data Persistence** - Settings and window state saved between sessions

### 📋 Included Shortcuts

The application comes pre-loaded with shortcuts for:

- **Windows System** (10 shortcuts) - Win+L, Win+D, Alt+Tab, etc.
- **Text Editing** (8 shortcuts) - Ctrl+Shift+K, Ctrl+D, Alt+Up/Down, etc.
- **Browser** (9 shortcuts) - Ctrl+T, Ctrl+W, Ctrl+Shift+T, etc.
- **VS Code** (8 shortcuts) - Ctrl+Shift+P, Ctrl+P, Ctrl+Shift+E, etc.
- **Office** (8 shortcuts) - Ctrl+N, Ctrl+S, Ctrl+B, etc.

## 🛠️ Technical Details

### Requirements
- Windows 10/11
- .NET 8.0 Runtime

### Architecture
- **Framework**: WPF with C# (.NET 8)
- **Pattern**: MVVM with ViewModels, Services, and Models
- **Data Storage**: JSON files for shortcuts and settings
- **UI**: Responsive grid layout with search/filter capabilities

### Key Components

#### Services Layer
- `HotkeyService` - Global hotkey registration using Win32 APIs
- `TrayService` - System tray integration with notifications
- `ShortcutDataService` - Data persistence and management

#### ViewModels
- `MainViewModel` - Main window data binding and commands
- `SettingsViewModel` - Settings configuration and persistence

#### Models
- `Shortcut` - Individual keyboard shortcut data
- `Category` - Shortcut categorization
- `Settings` - Application configuration

## 🚀 Getting Started

### Installation
1. Download the latest release from the [Releases](../../releases) page
2. Extract the files to your desired location
3. Run `PowerHotkeys.exe`
4. The application will start in the system tray

### Usage
1. **Show/Hide**: Press `Ctrl + Alt + K` or double-click the tray icon
2. **Search**: Type in the search box to filter shortcuts
3. **Filter**: Use the category dropdown to show specific types
4. **Settings**: Click the Settings button to customize behavior
5. **Exit**: Right-click tray icon and select Exit

## 🎯 Roadmap

### Future Enhancements
- Plugin system for custom shortcut sources
- Cloud sync for shortcut collections
- Usage analytics and personalized recommendations
- Integration with popular applications (VS Code extensions, etc.)
- Cross-platform support (macOS, Linux)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 🏗️ Building from Source

### Prerequisites
- Visual Studio 2022 or VS Code with C# extension
- .NET 8.0 SDK
- Windows 10/11 for testing

### Build Steps
```bash
git clone https://github.com/DontDoThat21/PowerKeys.git
cd PowerKeys/PowerHotkeys
dotnet restore
dotnet build
dotnet run
```

### Project Structure
```
PowerHotkeys/
├── Models/           # Data models
├── ViewModels/       # MVVM ViewModels
├── Views/           # WPF Windows/UserControls
├── Services/        # Business logic services
├── Resources/       # JSON data, icons, etc.
└── App.xaml        # Application entry point
```

## 📞 Support

If you encounter any issues or have questions:
- Create an [Issue](../../issues) on GitHub
- Check the [Wiki](../../wiki) for documentation
- Review [Discussions](../../discussions) for community help

---

**PowerKeys** - Making keyboard shortcuts accessible for power users everywhere! ⌨️✨