using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardAI.ViewModels.History;
using ClipboardAI.ViewModels.Settings;

namespace ClipboardAI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public HistoryViewModel HistoryViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public MainViewModel(HistoryViewModel historyViewModel, SettingsViewModel settingsViewModel)
    {
        HistoryViewModel = historyViewModel;
        SettingsViewModel = settingsViewModel;
    }
}
