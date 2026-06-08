using System;
using System.Windows;
using System.Windows.Interop;

namespace ClipboardAI.Views.Popups;

public partial class ClipboardPopup : Window
{
    public ClipboardPopup(ViewModels.MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        // Hide when losing focus
        this.Hide();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = ClipboardAI.Infrastructure.ServiceLocator.GetService<ClipboardAI.Views.Windows.MainWindow>();
        mainWindow.Show();
        mainWindow.Activate();
        this.Hide(); // Hide popup when opening dashboard
    }

    public void ShowAtCursor()
    {
        // Simple way to get cursor position in WPF without Forms
        // For accurate multi-monitor we usually use Win32 GetCursorPos, 
        // but for now placing it roughly centered or using simple screen bounds
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.Show();
        this.Activate();
    }
}
