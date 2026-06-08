namespace ClipboardAI.Models;

public class UserSettings
{
    public int MaxHistoryItems { get; set; } = 50;
    public bool LaunchAtStartup { get; set; } = false;
    public bool StartAsAdmin { get; set; } = false;
    public string AppTheme { get; set; } = "Windows default";
    public bool EnableKeyboardManager { get; set; } = true;
    public string OpenPopupHotkey { get; set; } = "Ctrl+Shift+V";
    public string ToggleBatchCopyHotkey { get; set; } = "Ctrl+Shift+B";
}
