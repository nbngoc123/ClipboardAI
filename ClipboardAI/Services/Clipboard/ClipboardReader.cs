using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using System.Windows;
using ClipboardAI.Models;
using ClipboardAI.Helpers;

namespace ClipboardAI.Services.Clipboard;

public static class ClipboardReader
{
    public static string ReadText()
    {
        if (System.Windows.Clipboard.ContainsText())
            return System.Windows.Clipboard.GetText();
        return string.Empty;
    }

    public static StringCollection? ReadFileDropList()
    {
        if (System.Windows.Clipboard.ContainsFileDropList())
            return System.Windows.Clipboard.GetFileDropList();
        return null;
    }

    private static string ReadImage()
    {
        if (System.Windows.Clipboard.ContainsImage())
        {
            var image = System.Windows.Clipboard.GetImage();
            if (image == null) return string.Empty;

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            
            string fileName = Guid.NewGuid().ToString() + ".png";
            string filePath = Path.Combine(AppPaths.ImagesFolder, fileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
            return filePath;
        }
        return string.Empty;
    }

    public static string ReadContent(ClipboardContentType type)
    {
        return type switch
        {
            ClipboardContentType.Text => ReadText(),
            ClipboardContentType.Html => System.Windows.Clipboard.GetText(TextDataFormat.Html),
            ClipboardContentType.RichText => System.Windows.Clipboard.GetText(TextDataFormat.Rtf),
            ClipboardContentType.FilePath => ReadFileDropList()?[0] ?? string.Empty,
            ClipboardContentType.Image => ReadImage(),
            _ => string.Empty
        };
    }
}
