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
            case "SnippingOcr":
                System.Windows.Application.Current.Dispatcher.Invoke(async () => 
                {
                    var window = new Views.Windows.SnippingWindow();
                    if (window.ShowDialog() == true && window.CapturedImage != null)
                    {
                        try
                        {
                            var ocrService = ClipboardAI.Infrastructure.ServiceLocator.GetService<ClipboardAI.Services.OCR.IOcrService>();
                            var text = await ocrService.ExtractTextFromImageAsync(window.CapturedImage);
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                System.Windows.Clipboard.SetText(text);
                                _trayIconService.ShowNotification("Snipping OCR", "Text extracted and copied to clipboard!");
                            }
                            else
                            {
                                _trayIconService.ShowNotification("Snipping OCR", "No text found in the selected region.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _trayIconService.ShowNotification("OCR Error", ex.Message);
                        }
                    }
                });
                break;
            case "PasteNextBatchItem":
                var item = _clipboardService.GetNextBatchItem();
                if (item != null)
                {
                    // Notify user of the item being pasted (optional)
                    var remaining = _clipboardService.GetBatchQueueCount();
                    _trayIconService.ShowNotification("Batch Paste", $"Pasting item... ({remaining} items remaining)");
                    
                    // Set clipboard FIRST, then paste after delay
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (item.ContentType == Models.ClipboardContentType.Image)
                        {
                            try
                            {
                                var bitmap = new System.Drawing.Bitmap(item.Content);
                                System.Windows.Clipboard.SetImage(
                                    System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                        bitmap.GetHbitmap(),
                                        IntPtr.Zero,
                                        System.Windows.Int32Rect.Empty,
                                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()));
                                bitmap.Dispose();
                            }
                            catch
                            {
                                _trayIconService.ShowNotification("Batch Paste", "Cannot paste image item.");
                                return;
                            }
                        }
                        else
                        {
                            System.Windows.Clipboard.SetText(item.Content);
                        }
                    });
                    SimulatePaste();
                }
                else
                {
                    _trayIconService.ShowNotification("Batch Paste", "No more items in batch queue. Use Ctrl+Shift+B to record a new batch.");
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
