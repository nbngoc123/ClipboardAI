using System;
using ClipboardAI.Models;

namespace ClipboardAI.Services.Clipboard;

public interface IClipboardService
{
    event EventHandler<ClipboardItem>? ClipboardChanged;
    void Start();
    void Stop();
    
    // Batch Copy Methods
    void StartBatchRecording();
    void StopBatchRecording();
    ClipboardItem? GetNextBatchItem();
    bool IsBatchRecording { get; }
}
