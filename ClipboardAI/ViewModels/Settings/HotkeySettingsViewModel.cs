using ClipboardAI.Services.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClipboardAI.ViewModels.Settings;

public partial class HotkeySettingsViewModel : ObservableObject
{
    private readonly SettingsManager _settingsManager;
    private readonly ClipboardAI.Services.Hotkey.HotkeyRegistrar _hotkeyRegistrar;

    [ObservableProperty]
    private bool _enableKeyboardManager;

    [ObservableProperty]
    private string _openPopupHotkey;

    [ObservableProperty]
    private string _toggleBatchCopyHotkey;

    public HotkeySettingsViewModel(SettingsManager settingsManager, ClipboardAI.Services.Hotkey.HotkeyRegistrar hotkeyRegistrar)
    {
        _settingsManager = settingsManager;
        _hotkeyRegistrar = hotkeyRegistrar;
        _enableKeyboardManager = _settingsManager.CurrentSettings.EnableKeyboardManager;
        _openPopupHotkey = _settingsManager.CurrentSettings.OpenPopupHotkey;
        _toggleBatchCopyHotkey = _settingsManager.CurrentSettings.ToggleBatchCopyHotkey;
    }

    partial void OnEnableKeyboardManagerChanged(bool value)
    {
        _settingsManager.CurrentSettings.EnableKeyboardManager = value;
        _settingsManager.SaveSettings();
    }

    [RelayCommand]
    private void EditOpenPopupHotkey()
    {
        var dialog = new ClipboardAI.Views.Popups.EditHotkeyWindow(OpenPopupHotkey);
        if (dialog.ShowDialog() == true)
        {
            OpenPopupHotkey = dialog.HotkeyString;
            _settingsManager.CurrentSettings.OpenPopupHotkey = dialog.HotkeyString;
            _settingsManager.SaveSettings();
            _hotkeyRegistrar.RegisterDefaultHotkeys();
        }
    }

    [RelayCommand]
    private void EditToggleBatchCopyHotkey()
    {
        var dialog = new ClipboardAI.Views.Popups.EditHotkeyWindow(ToggleBatchCopyHotkey);
        if (dialog.ShowDialog() == true)
        {
            ToggleBatchCopyHotkey = dialog.HotkeyString;
            _settingsManager.CurrentSettings.ToggleBatchCopyHotkey = dialog.HotkeyString;
            _settingsManager.SaveSettings();
            _hotkeyRegistrar.RegisterDefaultHotkeys();
        }
    }
}
