using System.Windows.Input;

namespace ClipboardAI.Services.Hotkey;

public class HotkeyRegistrar
{
    private readonly IHotkeyService _hotkeyService;
    private readonly ClipboardAI.Services.Settings.SettingsManager _settingsManager;

    public HotkeyRegistrar(IHotkeyService hotkeyService, ClipboardAI.Services.Settings.SettingsManager settingsManager)
    {
        _hotkeyService = hotkeyService;
        _settingsManager = settingsManager;
    }

    public void RegisterDefaultHotkeys()
    {
        UnregisterAll();
        var converter = new KeyGestureConverter();
        
        try {
            if (!string.IsNullOrEmpty(_settingsManager.CurrentSettings.OpenPopupHotkey)) {
                var gesture = (KeyGesture)converter.ConvertFromString(_settingsManager.CurrentSettings.OpenPopupHotkey)!;
                _hotkeyService.Register("OpenPopup", gesture.Key, gesture.Modifiers);
            }
        } catch { /* Ignore invalid */ }

        try {
            if (!string.IsNullOrEmpty(_settingsManager.CurrentSettings.ToggleBatchCopyHotkey)) {
                var gesture = (KeyGesture)converter.ConvertFromString(_settingsManager.CurrentSettings.ToggleBatchCopyHotkey)!;
                _hotkeyService.Register("ToggleBatchCopy", gesture.Key, gesture.Modifiers);
            }
        } catch { /* Ignore invalid */ }
        
        try {
            if (!string.IsNullOrEmpty(_settingsManager.CurrentSettings.SnippingOcrHotkey)) {
                var gesture = (KeyGesture)converter.ConvertFromString(_settingsManager.CurrentSettings.SnippingOcrHotkey)!;
                _hotkeyService.Register("SnippingOcr", gesture.Key, gesture.Modifiers);
            }
        } catch { /* Ignore invalid */ }
        
        try {
            if (!string.IsNullOrEmpty(_settingsManager.CurrentSettings.PasteNextBatchItemHotkey)) {
                var gesture = (KeyGesture)converter.ConvertFromString(_settingsManager.CurrentSettings.PasteNextBatchItemHotkey)!;
                _hotkeyService.Register("PasteNextBatchItem", gesture.Key, gesture.Modifiers);
            }
        } catch { /* Ignore invalid */ }

        // Ctrl+1..9 -> Paste Slot
        _hotkeyService.Register("PasteSlot1", Key.D1, ModifierKeys.Control);
        _hotkeyService.Register("PasteSlot2", Key.D2, ModifierKeys.Control);
        _hotkeyService.Register("PasteSlot3", Key.D3, ModifierKeys.Control);
    }

    public void UnregisterAll()
    {
        _hotkeyService.Unregister("OpenPopup");
        _hotkeyService.Unregister("ToggleBatchCopy");
        _hotkeyService.Unregister("SnippingOcr");
        _hotkeyService.Unregister("PasteNextBatchItem");
        _hotkeyService.Unregister("PasteSlot1");
        _hotkeyService.Unregister("PasteSlot2");
        _hotkeyService.Unregister("PasteSlot3");
    }
}
