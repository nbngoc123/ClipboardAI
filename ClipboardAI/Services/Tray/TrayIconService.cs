using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Controls;
using System;
using ClipboardAI.Infrastructure;
using ClipboardAI.Views.Popups;
using ClipboardAI.Services.Clipboard;

namespace ClipboardAI.Services.Tray;

public class TrayIconService : ITrayIconService, IDisposable
{
    private TaskbarIcon? _taskbarIcon;

    public void Show()
    {
        if (_taskbarIcon == null)
        {
            _taskbarIcon = new TaskbarIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location),
                ToolTipText = "ClipboardAI is running"
            };

            var menu = new ContextMenu();
            
            var openItem = new MenuItem { Header = "Open History" };
            openItem.Click += (s, e) => ServiceLocator.GetService<ClipboardPopup>().ShowAtCursor();
            
            var toggleBatchItem = new MenuItem { Header = "Toggle Batch Copy (Ctrl+Shift+B)" };
            toggleBatchItem.Click += (s, e) => 
            {
                var clipboard = ServiceLocator.GetService<IClipboardService>();
                if (clipboard.IsBatchRecording) clipboard.StopBatchRecording();
                else clipboard.StartBatchRecording();
            };

            var settingsItem = new MenuItem { Header = "Open Dashboard" };
            settingsItem.Click += (s, e) => 
            {
                var mainWindow = ServiceLocator.GetService<ClipboardAI.Views.Windows.MainWindow>();
                mainWindow.Show();
                mainWindow.Activate();
            };

            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (s, e) => Application.Current.Shutdown();
            
            menu.Items.Add(openItem);
            menu.Items.Add(toggleBatchItem);
            menu.Items.Add(settingsItem);
            menu.Items.Add(new Separator());
            menu.Items.Add(exitItem);
            
            _taskbarIcon.ContextMenu = menu;
            
            // Double click opens Dashboard
            _taskbarIcon.TrayMouseDoubleClick += (s, e) => {
                var mainWindow = ServiceLocator.GetService<ClipboardAI.Views.Windows.MainWindow>();
                mainWindow.Show();
                mainWindow.Activate();
            };
        }
        _taskbarIcon.Visibility = Visibility.Visible;
    }

    public void Hide()
    {
        if (_taskbarIcon != null)
        {
            _taskbarIcon.Visibility = Visibility.Hidden;
        }
    }

    public void UpdateTooltip(string text)
    {
        if (_taskbarIcon != null)
        {
            _taskbarIcon.ToolTipText = text;
        }
    }

    public void ShowNotification(string title, string message)
    {
        if (_taskbarIcon != null)
        {
            _taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Info);
        }
    }

    public void Dispose()
    {
        _taskbarIcon?.Dispose();
    }
}
