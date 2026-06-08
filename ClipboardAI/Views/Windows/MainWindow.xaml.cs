using System.ComponentModel;
using System.Windows;
using ClipboardAI.Services.Tray;

namespace ClipboardAI.Views.Windows;

public partial class MainWindow : Window
{
    private readonly ITrayIconService _trayIconService;

    public MainWindow(ITrayIconService trayIconService)
    {
        InitializeComponent();
        _trayIconService = trayIconService;
        _trayIconService.Show();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // Cancel the close operation
        e.Cancel = true;
        
        // Hide the window instead
        this.Hide();
        
        _trayIconService.ShowNotification("ClipboardAI is running", "The app is minimized. Press Ctrl+Shift+V to use the popup.");
    }
}
