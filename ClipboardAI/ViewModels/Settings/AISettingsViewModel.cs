using ClipboardAI.Models;
using ClipboardAI.Services.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClipboardAI.ViewModels.Settings;

public partial class AISettingsViewModel : ObservableObject
{
    private readonly SettingsManager _settingsManager;

    [ObservableProperty]
    private string _endpoint;

    [ObservableProperty]
    private string _token;

    [ObservableProperty]
    private string _modelName;

    public AISettingsViewModel(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        var currentSettings = _settingsManager.CurrentSettings;
        
        _endpoint = currentSettings.AIEndpoint ?? string.Empty;
        _token = currentSettings.AIToken ?? string.Empty;
        _modelName = currentSettings.AIModelName ?? string.Empty;
    }

    [RelayCommand]
    private void SaveSettings()
    {
        var settings = _settingsManager.CurrentSettings;
        settings.AIEndpoint = Endpoint;
        settings.AIToken = Token;
        settings.AIModelName = ModelName;
        _settingsManager.SaveSettings();
    }
}
