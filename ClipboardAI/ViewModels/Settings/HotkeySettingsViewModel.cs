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

    [ObservableProperty]
    private string _pasteNextBatchItemHotkey;

    [ObservableProperty]
    private string _snippingOcrHotkey;

    public HotkeySettingsViewModel(SettingsManager settingsManager, ClipboardAI.Services.Hotkey.HotkeyRegistrar hotkeyRegistrar)
    {
        _settingsManager = settingsManager;
        _hotkeyRegistrar = hotkeyRegistrar;
        var s = _settingsManager.CurrentSettings;
        _enableKeyboardManager = s.EnableKeyboardManager;
        _openPopupHotkey = s.OpenPopupHotkey;
        _toggleBatchCopyHotkey = s.ToggleBatchCopyHotkey;
        _pasteNextBatchItemHotkey = string.IsNullOrEmpty(s.PasteNextBatchItemHotkey) ? "Ctrl+Shift+X" : s.PasteNextBatchItemHotkey;
        _snippingOcrHotkey = string.IsNullOrEmpty(s.SnippingOcrHotkey) ? "Ctrl+Shift+O" : s.SnippingOcrHotkey;
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

    [RelayCommand]
    private void EditPasteNextBatchItemHotkey()
    {
        var dialog = new ClipboardAI.Views.Popups.EditHotkeyWindow(PasteNextBatchItemHotkey);
        if (dialog.ShowDialog() == true)
        {
            PasteNextBatchItemHotkey = dialog.HotkeyString;
            _settingsManager.CurrentSettings.PasteNextBatchItemHotkey = dialog.HotkeyString;
            _settingsManager.SaveSettings();
            _hotkeyRegistrar.RegisterDefaultHotkeys();
        }
    }

    [RelayCommand]
    private void EditSnippingOcrHotkey()
    {
        var dialog = new ClipboardAI.Views.Popups.EditHotkeyWindow(SnippingOcrHotkey);
        if (dialog.ShowDialog() == true)
        {
            SnippingOcrHotkey = dialog.HotkeyString;
            _settingsManager.CurrentSettings.SnippingOcrHotkey = dialog.HotkeyString;
            _settingsManager.SaveSettings();
            _hotkeyRegistrar.RegisterDefaultHotkeys();
        }
    }
}
