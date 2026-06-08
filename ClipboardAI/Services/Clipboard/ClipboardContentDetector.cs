using System.Windows;
using ClipboardAI.Models;

namespace ClipboardAI.Services.Clipboard;

public static class ClipboardContentDetector
{
    public static ClipboardContentType DetectType()
    {
        if (System.Windows.Clipboard.ContainsFileDropList())
            return ClipboardContentType.FilePath;
            
        if (System.Windows.Clipboard.ContainsImage())
            return ClipboardContentType.Image;

        if (System.Windows.Clipboard.ContainsText(TextDataFormat.UnicodeText) || 
            System.Windows.Clipboard.ContainsText(TextDataFormat.Text))
            return ClipboardContentType.Text;

        if (System.Windows.Clipboard.ContainsText(TextDataFormat.Html))
            return ClipboardContentType.Html;

        if (System.Windows.Clipboard.ContainsText(TextDataFormat.Rtf))
            return ClipboardContentType.RichText;

        return ClipboardContentType.Text; // default fallback
    }
}
