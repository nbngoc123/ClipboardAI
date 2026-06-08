# ClipboardAI

ClipboardAI is a modern, smart, and lightweight Windows clipboard manager built with C# WPF (.NET 10.0). Inspired by the Microsoft PowerToys design system, it provides a seamless, beautiful, and highly productive extension to your daily workflow.

## 🌟 Features

* **PowerToys-Inspired UI**: A sleek, dark-themed interface with glassmorphism, smooth animations, and rounded corners that looks perfectly native to Windows 11.
* **Smart Clipboard Tracking**: Monitors both text and images dynamically, using low-level OS sequence numbers to prevent duplicate files and resource leaks.
* **Batch Copy Mode**: Turn on Batch Copy (`Ctrl + Shift + B`) to queue up multiple copied items in sequence. Then, paste them one by one sequentially (`Ctrl + Shift + X`). Perfect for filling out forms or migrating data!
* **Global Hotkeys**: A fully customizable Keyboard Manager. Edit and bind your own shortcuts (e.g., `Alt + V`, `Ctrl + Shift + V`) which register seamlessly with the Windows OS.
* **Floating Popup History**: Press your custom hotkey anywhere in Windows to bring up a non-intrusive floating popup to quickly search and select your clipboard history.
* **Database Persistence**: Uses SQLite to reliably store your history, ensuring your copied items survive reboots.
* **Custom Limits & Startup**: Define maximum history slots and configure the app to launch quietly in the system tray at Windows startup.

## 🚀 Getting Started

### Prerequisites
* .NET 10.0 SDK
* Visual Studio 2022 (or later)

### Build and Run
1. Clone the repository.
2. Open `ClipboardAI.sln` in Visual Studio or use the .NET CLI.
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run --project ClipboardAI
   ```

## ⌨️ Default Hotkeys

* **Open Clipboard Popup**: `Ctrl + Shift + V`
* **Toggle Batch Mode**: `Ctrl + Shift + B`
* **Paste Next Batch Item**: `Ctrl + Shift + X`

*(All hotkeys can be remapped directly in the app's Settings menu).*

## 🛠️ Tech Stack

* **Framework**: WPF (.NET 10.0)
* **Architecture**: MVVM (CommunityToolkit.Mvvm)
* **Database**: SQLite (Microsoft.Data.Sqlite / Dapper)
* **Dependency Injection**: Microsoft.Extensions.DependencyInjection
* **System Hotkeys**: NHotkey.Wpf

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the issues page.

## 📄 License

This project is open-source and available under the MIT License.
