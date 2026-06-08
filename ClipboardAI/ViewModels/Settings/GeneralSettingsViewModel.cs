using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClipboardAI.Services.Settings;
using System.Diagnostics;
using Microsoft.Win32;

namespace ClipboardAI.ViewModels.Settings;

public partial class GeneralSettingsViewModel : ObservableObject
{
    private readonly SettingsManager _settingsManager;

    [ObservableProperty]
    private int _maxHistoryItems;

    [ObservableProperty]
    private bool _launchAtStartup;

    [ObservableProperty]
    private bool _startAsAdmin;

    [ObservableProperty]
    private string _appTheme = "Windows default";

    public GeneralSettingsViewModel(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        var s = _settingsManager.CurrentSettings;
        _maxHistoryItems = s.MaxHistoryItems;
        _launchAtStartup = s.LaunchAtStartup;
        _startAsAdmin = s.StartAsAdmin;
        _appTheme = s.AppTheme;
    }

    partial void OnMaxHistoryItemsChanged(int value) => SaveSettings();
    partial void OnLaunchAtStartupChanged(bool value)
    {
        UpdateRegistryStartup(value);
        SaveSettings();
    }
    partial void OnStartAsAdminChanged(bool value) => SaveSettings();
    partial void OnAppThemeChanged(string value) => SaveSettings();

    private void SaveSettings()
    {
        var s = _settingsManager.CurrentSettings;
        s.MaxHistoryItems = MaxHistoryItems;
        s.LaunchAtStartup = LaunchAtStartup;
        s.StartAsAdmin = StartAsAdmin;
        s.AppTheme = AppTheme;
        _settingsManager.SaveSettings();
    }

    private void UpdateRegistryStartup(bool enable)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null)
            {
                if (enable)
                {
                    string path = Process.GetCurrentProcess().MainModule?.FileName ?? "";
                    key.SetValue("ClipboardAI", path);
                }
                else
                {
                    key.DeleteValue("ClipboardAI", false);
                }
            }
        }
        catch { }
    }
}
