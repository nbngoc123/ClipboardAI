using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ClipboardAI.Models;
using ClipboardAI.Services.AI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClipboardAI.ViewModels.AI;

public partial class AIPanelViewModel : ObservableObject
{
    private readonly IAIService _aiService;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private string _targetContent = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasResult;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isVisible = false;

    public ObservableCollection<ExtractedField> ExtractedFields { get; } = new();

    public AIPanelViewModel(IAIService aiService)
    {
        _aiService = aiService;
    }

    public void OpenPanelForContent(string content)
    {
        TargetContent = content;
        IsVisible = true;
    }

    [RelayCommand]
    private async Task ExtractDataAsync()
    {
        if (string.IsNullOrWhiteSpace(TargetContent))
        {
            ErrorMessage = "Content is empty.";
            return;
        }

        IsLoading = true;
        HasResult = false;
        ErrorMessage = string.Empty;
        ExtractedFields.Clear();

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var result = await _aiService.ExtractDataAsync(TargetContent, _cancellationTokenSource.Token);
            
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var field in result)
                {
                    ExtractedFields.Add(field);
                }
                HasResult = ExtractedFields.Count > 0;
                if (!HasResult)
                {
                    ErrorMessage = "AI could not extract any structured fields from this content.";
                }
            });
        }
        catch (OperationCanceledException)
        {
            ErrorMessage = "Extraction canceled.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SummarizeAndTranslateAsync()
    {
        if (string.IsNullOrWhiteSpace(TargetContent))
        {
            ErrorMessage = "Content is empty.";
            return;
        }

        IsLoading = true;
        HasResult = false;
        ErrorMessage = string.Empty;
        ExtractedFields.Clear();

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var result = await _aiService.SummarizeAndTranslateAsync(TargetContent, _cancellationTokenSource.Token);
            
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var field in result)
                {
                    ExtractedFields.Add(field);
                }
                HasResult = ExtractedFields.Count > 0;
                if (!HasResult)
                {
                    ErrorMessage = "AI could not generate a summary and translation.";
                }
            });
        }
        catch (OperationCanceledException)
        {
            ErrorMessage = "Operation canceled.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void CopyField(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Clipboard.SetText(value);
        }
    }
}
