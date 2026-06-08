using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClipboardAI.Models;
using ClipboardAI.Data.Repositories;

namespace ClipboardAI.ViewModels.History;

public partial class HistoryViewModel : ObservableObject
{
    private readonly IClipboardRepository _repository;

    [ObservableProperty]
    private ObservableCollection<ClipboardItem> _items = new();

    public HistorySearchViewModel SearchViewModel { get; } = new();

    [ObservableProperty]
    private bool _isBatchRecording;

    private readonly ClipboardAI.Services.Settings.SettingsManager _settingsManager;

    public HistoryViewModel(IClipboardRepository repository, ClipboardAI.Services.Settings.SettingsManager settingsManager)
    {
        _repository = repository;
        _settingsManager = settingsManager;
        SearchViewModel.SearchQueryChanged += OnSearchQueryChanged;
    }

    private async void OnSearchQueryChanged(object? sender, string query)
    {
        await LoadItemsAsync();
    }

    public async Task LoadItemsAsync()
    {
        int max = _settingsManager.CurrentSettings.MaxHistoryItems;
        var recentItems = await _repository.GetRecentAsync(max);
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            Items.Clear();
            foreach (var item in recentItems)
            {
                Items.Add(item);
            }
        });
    }

    public void AddNewItem(ClipboardItem item)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Items.Insert(0, item);
        });
    }

    public async void EnsureLimit(int maxItems)
    {
        await _repository.DeleteOldestAsync(maxItems);
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            while (Items.Count > maxItems)
            {
                Items.RemoveAt(Items.Count - 1);
            }
        });
    }

    [RelayCommand]
    private void CopyToClipboard(ClipboardItem item)
    {
        if (item == null || string.IsNullOrEmpty(item.Content)) return;
        try { System.Windows.Clipboard.SetText(item.Content); } catch { }
    }

    [RelayCommand]
    private async Task TogglePinItemAsync(ClipboardItem item)
    {
        if (item == null) return;
        item.IsPinned = item.IsPinned == 1 ? 0 : 1;
        // In a real app we'd update DB here: await _repository.UpdateAsync(item);
        await LoadItemsAsync(); // Reload to sort pinned first
    }

    [RelayCommand]
    private async Task DeleteItemAsync(ClipboardItem item)
    {
        if (item == null) return;
        Items.Remove(item);
        // await _repository.DeleteAsync(item.Id);
    }
}
