using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardAI.ViewModels.Settings;

public partial class SettingsViewModel : ObservableObject
{
    public GeneralSettingsViewModel GeneralSettings { get; }
    public HotkeySettingsViewModel HotkeySettings { get; }
    public AISettingsViewModel AISettings { get; }

    public SettingsViewModel(GeneralSettingsViewModel generalSettings, HotkeySettingsViewModel hotkeySettings, AISettingsViewModel aiSettings)
    {
        GeneralSettings = generalSettings;
        HotkeySettings = hotkeySettings;
        AISettings = aiSettings;
    }
}
