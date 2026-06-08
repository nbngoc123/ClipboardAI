using System.Collections.ObjectModel;
using ClipboardAI.Models;
using ClipboardAI.Services.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClipboardAI.ViewModels.Settings;

public partial class AIToolsConfigViewModel : ObservableObject
{
    private readonly SettingsManager _settingsManager;

    [ObservableProperty]
    private string _extractLanguage;

    [ObservableProperty]
    private string _extractCustomPrompt;

    [ObservableProperty]
    private string _summaryLanguage;

    [ObservableProperty]
    private string _translationLanguage;

    [ObservableProperty]
    private string _aiTone;

    public ObservableCollection<string> AvailableLanguages { get; } = new()
    {
        "Auto", "English", "Vietnamese", "Japanese", "Korean", "Chinese", "French", "Spanish"
    };

    public ObservableCollection<string> AvailableTones { get; } = new()
    {
        "Professional", "Casual", "Friendly", "Direct", "Academic"
    };

    public AIToolsConfigViewModel(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        var currentSettings = _settingsManager.CurrentSettings;
        
        _extractLanguage = currentSettings.ExtractLanguage ?? "Auto";
        _extractCustomPrompt = currentSettings.ExtractCustomPrompt ?? string.Empty;
        _summaryLanguage = currentSettings.SummaryLanguage ?? "Vietnamese";
        _translationLanguage = currentSettings.TranslationLanguage ?? "Vietnamese";
        _aiTone = currentSettings.AITone ?? "Professional";
    }

    [RelayCommand]
    private void SaveSettings()
    {
        var settings = _settingsManager.CurrentSettings;
        settings.ExtractLanguage = ExtractLanguage;
        settings.ExtractCustomPrompt = ExtractCustomPrompt;
        settings.SummaryLanguage = SummaryLanguage;
        settings.TranslationLanguage = TranslationLanguage;
        settings.AITone = AiTone;
        _settingsManager.SaveSettings();
    }
}
