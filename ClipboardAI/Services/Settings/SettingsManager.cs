using System.IO;
using System.Text.Json;
using ClipboardAI.Helpers;
using ClipboardAI.Models;

namespace ClipboardAI.Services.Settings;

public class SettingsManager
{
    private readonly string _settingsFilePath;
    public UserSettings CurrentSettings { get; private set; }

    public SettingsManager()
    {
        _settingsFilePath = Path.Combine(AppPaths.AppDataFolder, "user_settings.json");
        LoadSettings();
    }

    public void LoadSettings()
    {
        var defaults = new UserSettings();
        if (File.Exists(_settingsFilePath))
        {
            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                var loaded = JsonSerializer.Deserialize<UserSettings>(json);
                if (loaded != null)
                {
                    // Merge: if a field is null/empty, fall back to the default value
                    // This handles the case where new settings fields are added in newer versions
                    CurrentSettings = new UserSettings
                    {
                        MaxHistoryItems = loaded.MaxHistoryItems,
                        LaunchAtStartup = loaded.LaunchAtStartup,
                        StartAsAdmin = loaded.StartAsAdmin,
                        AppTheme = string.IsNullOrEmpty(loaded.AppTheme) ? defaults.AppTheme : loaded.AppTheme,
                        EnableKeyboardManager = loaded.EnableKeyboardManager,
                        OpenPopupHotkey = string.IsNullOrEmpty(loaded.OpenPopupHotkey) ? defaults.OpenPopupHotkey : loaded.OpenPopupHotkey,
                        ToggleBatchCopyHotkey = string.IsNullOrEmpty(loaded.ToggleBatchCopyHotkey) ? defaults.ToggleBatchCopyHotkey : loaded.ToggleBatchCopyHotkey,
                        PasteNextBatchItemHotkey = string.IsNullOrEmpty(loaded.PasteNextBatchItemHotkey) ? defaults.PasteNextBatchItemHotkey : loaded.PasteNextBatchItemHotkey,
                        SnippingOcrHotkey = string.IsNullOrEmpty(loaded.SnippingOcrHotkey) ? defaults.SnippingOcrHotkey : loaded.SnippingOcrHotkey,
                        AIEndpoint = string.IsNullOrEmpty(loaded.AIEndpoint) ? defaults.AIEndpoint : loaded.AIEndpoint,
                        AIToken = loaded.AIToken ?? defaults.AIToken,
                        AIModelName = string.IsNullOrEmpty(loaded.AIModelName) ? defaults.AIModelName : loaded.AIModelName,
                        ExtractLanguage = string.IsNullOrEmpty(loaded.ExtractLanguage) ? defaults.ExtractLanguage : loaded.ExtractLanguage,
                        ExtractCustomPrompt = loaded.ExtractCustomPrompt ?? defaults.ExtractCustomPrompt,
                        SummaryLanguage = string.IsNullOrEmpty(loaded.SummaryLanguage) ? defaults.SummaryLanguage : loaded.SummaryLanguage,
                        TranslationLanguage = string.IsNullOrEmpty(loaded.TranslationLanguage) ? defaults.TranslationLanguage : loaded.TranslationLanguage,
                        AITone = string.IsNullOrEmpty(loaded.AITone) ? defaults.AITone : loaded.AITone,
                    };
                }
                else
                {
                    CurrentSettings = defaults;
                }
            }
            catch
            {
                CurrentSettings = defaults;
            }
        }
        else
        {
            CurrentSettings = defaults;
        }
    }

    public void SaveSettings()
    {
        var json = JsonSerializer.Serialize(CurrentSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsFilePath, json);
    }
}
