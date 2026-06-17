using System;
using ClipboardAI.Models;

namespace ClipboardAI.Services.Clipboard;

public class ClipboardService : IClipboardService
{
    private readonly ClipboardPoller _poller;
    private readonly BatchCopyMode _batchMode = new();

    public event EventHandler<ClipboardItem>? ClipboardChanged;

    public bool IsBatchRecording => _batchMode.IsRecording;

    public ClipboardService()
    {
        _poller = new ClipboardPoller();
        _poller.OnNewItemDetected += OnNewItemDetected;
    }

    private void OnNewItemDetected(object? sender, ClipboardItem item)
    {
        if (_batchMode.IsRecording)
        {
            _batchMode.Queue.Enqueue(item);
        }
        
        ClipboardChanged?.Invoke(this, item);
    }

    public void Start() => _poller.Start();
    public void Stop() => _poller.Stop();

    public void StartBatchRecording()
    {
        _batchMode.IsRecording = true;
        _batchMode.Queue.Clear();
    }

    public void StopBatchRecording()
    {
        _batchMode.IsRecording = false;
    }

    public ClipboardItem? GetNextBatchItem()
    {
        if (_batchMode.Queue.Count > 0)
        {
            return _batchMode.Queue.Dequeue();
        }
        return null;
    }

    public int GetBatchQueueCount() => _batchMode.Queue.Count;
}
