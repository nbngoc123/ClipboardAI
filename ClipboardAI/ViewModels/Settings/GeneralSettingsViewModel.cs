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

    [RelayCommand]
    private void UninstallAndExit()
    {
        var result = System.Windows.MessageBox.Show("Bạn có chắc chắn muốn xóa vĩnh viễn toàn bộ lịch sử copy, hình ảnh và cài đặt không?\nThao tác này không thể hoàn tác!", "Uninstall & Clean Data", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            UpdateRegistryStartup(false);
            
            // 1. Hủy đăng ký tất cả các Phím tắt hệ thống để giải phóng bộ nhớ.
            try { NHotkey.Wpf.HotkeyManager.Current.Remove("OpenPopup"); } catch { }
            try { NHotkey.Wpf.HotkeyManager.Current.Remove("ToggleBatchCopy"); } catch { }
            
            // 2 & 3. Xóa dữ liệu SQLite và Images
            string appDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClipboardAI");
            try 
            {
                if (System.IO.Directory.Exists(appDataPath))
                {
                    var psi = new ProcessStartInfo("cmd.exe", $"/c ping localhost -n 3 > nul & rmdir /s /q \"{appDataPath}\"")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    Process.Start(psi);
                }
            } 
            catch { }
            
            // 4. Hiển thị thông báo "Đã dọn dẹp sạch sẽ!"
            System.Windows.MessageBox.Show("Đã dọn dẹp sạch sẽ toàn bộ dữ liệu cấu hình và lịch sử!\n\nỨng dụng sẽ tự động thoát ngay bây giờ. Bạn có thể an tâm xóa file .exe đi.", "Hoàn tất Gỡ bỏ", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            
            System.Windows.Application.Current.Shutdown();
        }
    }
}
