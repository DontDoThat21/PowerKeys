# PowerUser Keys - MVP Implementation

A Windows desktop application that displays a grid of useful keyboard shortcuts for power users. The app runs silently in the background, accessible via system tray, and can be instantly shown/hidden using a global hotkey combination.

## Features Implemented (MVP P0)

### ✅ System Tray Integration
- App minimizes to system tray on startup
- Right-click context menu with Show/Hide/Exit options
- Double-click tray icon to show/hide window

### ✅ Global Hotkey Support
- **Ctrl + Alt + K** shows/hides the main window
- Works regardless of which application has focus
- Graceful handling of hotkey registration failures

### ✅ Grid Display
- Shows **41 pre-defined shortcuts** in organized grid
- **5 categories**: Windows System, Text Editing, Browser, File Management, Developer Tools
- Responsive 4-column layout (adjusts to window resize)
- Clear visual hierarchy: Application → Shortcut Keys → Description

### ✅ Window Management
- Window appears centered on screen
- Always stays on top when visible
- Fade in/out animations (200ms duration)
- Modern, borderless design with rounded corners

### ✅ Search and Filter
- Real-time search by keyword
- Category-based filtering
- Search across keys, descriptions, applications, and categories

## Technical Architecture

### Project Structure
```
PowerHotkeysWPF/
├── Models/
│   ├── Category.cs          # Category data model
│   └── Shortcut.cs          # Shortcut data model
├── Services/
│   ├── HotkeyService.cs     # Global hotkey registration
│   ├── TrayService.cs       # System tray integration
│   └── ShortcutDataService.cs # Data management & persistence
├── ViewModels/
│   └── MainViewModel.cs     # MVVM ViewModel for main window
├── Resources/
│   └── shortcuts.json       # Default shortcuts database
├── Tests/                   # Console test project
└── App files (MainWindow.xaml, App.xaml, etc.)
```

### Technologies Used
- **.NET 8.0** with WPF
- **MVVM Pattern** for clean architecture
- **Newtonsoft.Json** for data serialization
- **Hardcodet.NotifyIcon.Wpf** for system tray
- **Win32 API** for global hotkeys

## Default Shortcuts Database

The application comes with **41 essential shortcuts** across **5 categories**:

### Windows System (10 shortcuts)
- Win + L (Lock Screen)
- Win + D (Show Desktop)
- Alt + Tab (Switch Apps)
- And more...

### Text Editing (8 shortcuts)
- Ctrl + Shift + K (Delete Line)
- Ctrl + D (Duplicate Line)
- Alt + Up/Down (Move Line)
- And more...

### Browser (8 shortcuts)
- Ctrl + T (New Tab)
- Ctrl + W (Close Tab)
- Ctrl + Shift + T (Reopen Closed Tab)
- And more...

### File Management (7 shortcuts)
- Ctrl + C (Copy)
- Ctrl + V (Paste)
- F2 (Rename)
- And more...

### Developer Tools (8 shortcuts)
- Ctrl + ` (Toggle Terminal)
- Ctrl + Shift + P (Command Palette)
- F12 (Go to Definition)
- And more...

## Building and Running

### Prerequisites
- .NET 8.0 SDK
- Windows 10/11

### Build Instructions
```bash
cd PowerHotkeysWPF
dotnet restore
dotnet build
```

### Testing Core Components
```bash
cd PowerHotkeysWPF/Tests
dotnet run
```

### Running the Application
```bash
cd PowerHotkeysWPF
dotnet run
```

**Note**: The WPF application requires a Windows environment with desktop support. The core functionality has been tested and verified.

## Key Features

### Global Hotkey (Ctrl + Alt + K)
- Uses Win32 RegisterHotKey API
- Handles conflicts gracefully
- Works across all applications

### System Tray Integration
- Custom tray icon and tooltip
- Context menu with essential actions
- Balloon notifications for important messages

### Modern UI Design
- Borderless window with rounded corners
- Card-based layout for shortcuts
- Hover effects and visual feedback
- Smooth fade animations

### Data Management
- JSON-based storage for easy customization
- Search and filter functionality
- Category-based organization
- Extensible for custom shortcuts

## Future Enhancements (Post-MVP)

- Settings panel for customization
- Custom shortcut addition/editing
- Import/export functionality
- Themes and appearance options
- Startup with Windows
- Plugin system for external shortcuts

## Architecture Decisions

1. **MVVM Pattern**: Clean separation of concerns
2. **Service Layer**: Modular, testable components
3. **JSON Storage**: Human-readable, easily editable
4. **Dependency Injection**: Future-ready architecture
5. **Async/Await**: Responsive UI operations

## Testing

Core components have been tested:
- ✅ Shortcut data loading (41 shortcuts, 5 categories)
- ✅ Search functionality
- ✅ Category filtering
- ✅ JSON persistence

The application is ready for deployment on Windows systems with proper WPF support.