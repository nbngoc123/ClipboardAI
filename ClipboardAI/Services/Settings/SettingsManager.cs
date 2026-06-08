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
        if (File.Exists(_settingsFilePath))
        {
            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                CurrentSettings = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
            }
            catch
            {
                CurrentSettings = new UserSettings();
            }
        }
        else
        {
            CurrentSettings = new UserSettings();
        }
    }

    public void SaveSettings()
    {
        var json = JsonSerializer.Serialize(CurrentSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsFilePath, json);
    }
}
