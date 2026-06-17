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
    public string PasteNextBatchItemHotkey { get; set; } = "Ctrl+Shift+X";
    public string SnippingOcrHotkey { get; set; } = "Ctrl+Shift+O";
    
    // AI Settings
    public string AIEndpoint { get; set; } = "https://aistudio1303-resource.services.ai.azure.com/";
    public string AIToken { get; set; } = "";
    public string AIModelName { get; set; } = "gpt-4.1-mini";

    // AI Tool Behaviors
    public string ExtractLanguage { get; set; } = "Auto";
    public string ExtractCustomPrompt { get; set; } = "";
    public string SummaryLanguage { get; set; } = "Vietnamese";
    public string TranslationLanguage { get; set; } = "Vietnamese";
    public string AITone { get; set; } = "Professional";
}
