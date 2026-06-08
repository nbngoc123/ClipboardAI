using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Timers;

namespace ClipboardAI.ViewModels.History;

public partial class HistorySearchViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public event EventHandler<string>? SearchQueryChanged;

    private readonly System.Timers.Timer _debounceTimer;

    public HistorySearchViewModel()
    {
        _debounceTimer = new System.Timers.Timer(300);
        _debounceTimer.AutoReset = false;
        _debounceTimer.Elapsed += (s, e) => 
        {
            SearchQueryChanged?.Invoke(this, SearchQuery);
        };
    }

    partial void OnSearchQueryChanged(string value)
    {
        _debounceTimer.Stop();
        _debounceTimer.Start();
    }
}
