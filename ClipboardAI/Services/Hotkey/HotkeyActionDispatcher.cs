using System;
using System.Windows;
using ClipboardAI.Services.Clipboard;

using ClipboardAI.Services.Tray;

namespace ClipboardAI.Services.Hotkey;

public class HotkeyActionDispatcher
{
    private readonly IClipboardService _clipboardService;
    private readonly ITrayIconService _trayIconService;

    public HotkeyActionDispatcher(IHotkeyService hotkeyService, IClipboardService clipboardService, ITrayIconService trayIconService)
    {
        _clipboardService = clipboardService;
        _trayIconService = trayIconService;
        hotkeyService.OnHotkeyPressed += HandleHotkey;
    }

    private void HandleHotkey(object? sender, string actionName)
    {
        switch (actionName)
        {
            case "OpenPopup":
                ClipboardAI.Infrastructure.ServiceLocator.GetService<ClipboardAI.Views.Popups.ClipboardPopup>().ShowAtCursor();
                break;
            case "ToggleBatchCopy":
                if (_clipboardService.IsBatchRecording)
                {
                    _clipboardService.StopBatchRecording();
                    _trayIconService.ShowNotification("Batch Copy", "Batch Recording Stopped.");
                }
                else
                {
                    _clipboardService.StartBatchRecording();
                    _trayIconService.ShowNotification("Batch Copy", "🔴 Batch Recording Started! Copied items will be queued.");
                }
                break;
            case "PasteNextBatchItem":
                var item = _clipboardService.GetNextBatchItem();
                if (item != null)
                {
                    if (item.ContentType == Models.ClipboardContentType.Image)
                    {
                        var bmp = new System.Windows.Media.Imaging.BitmapImage(new Uri(item.Content));
                        System.Windows.Clipboard.SetImage(bmp);
                    }
                    else
                    {
                        System.Windows.Clipboard.SetText(item.Content);
                    }
                    SimulatePaste();
                }
                break;
            case "PasteSlot1":
                // TODO: Get pinned item 1 and paste
                break;
        }
    }

    private async void SimulatePaste()
    {
        // Wait 400ms so the user can release the Ctrl and Shift keys physically.
        // Otherwise, SendKeys "^v" combines with the physical Shift and produces Ctrl+Shift+V
        // which accidentally opens the popup or fails to paste.
        await System.Threading.Tasks.Task.Delay(400);
        
        System.Windows.Application.Current.Dispatcher.Invoke(() => 
        {
            System.Windows.Forms.SendKeys.SendWait("^v");
        });
    }
}
