using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using ClipboardAI.Models;

namespace ClipboardAI.Services.Clipboard;

public class ClipboardPoller
{
    [DllImport("user32.dll")]
    private static extern uint GetClipboardSequenceNumber();

    private readonly DispatcherTimer _timer;
    private string _lastHash = string.Empty;
    private uint _lastSequenceNumber;

    public event EventHandler<ClipboardItem>? OnNewItemDetected;

    public ClipboardPoller()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _timer.Tick += Timer_Tick;
        _lastSequenceNumber = GetClipboardSequenceNumber();
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    private void Timer_Tick(object? sender, EventArgs e)
    {
        try
        {
            uint currentSequence = GetClipboardSequenceNumber();
            if (currentSequence == _lastSequenceNumber) return; // Không có gì mới trên Clipboard

            _lastSequenceNumber = currentSequence;

            var item = ClipboardItemFactory.CreateFromCurrentClipboard();
            if (item != null)
            {
                var hash = ClipboardHasher.ComputeHash(item.Content);
                // Với ảnh, item.Content là đường dẫn file mới tạo, hash luôn khác nhau.
                // Nhưng nhờ có sequence number, ta đã lọc được việc lặp lại liên tục.
                if (hash != _lastHash || item.ContentType == ClipboardContentType.Image)
                {
                    _lastHash = hash;
                    OnNewItemDetected?.Invoke(this, item);
                }
            }
        }
        catch (Exception)
        {
            // Ignore clipboard access exceptions (e.g., locked by other app)
        }
    }
}
