using System;
using ClipboardAI.Models;

namespace ClipboardAI.Services.Clipboard;

public static class ClipboardItemFactory
{
    public static ClipboardItem? CreateFromCurrentClipboard()
    {
        var type = ClipboardContentDetector.DetectType();
        var content = ClipboardReader.ReadContent(type);

        if (string.IsNullOrWhiteSpace(content) || content == "[Image Data Placeholder]")
        {
            // Skip empty or currently unsupported formats for Phase 1 basic run
            if (type != ClipboardContentType.Image) // Let image pass for now with placeholder
            {
                return null;
            }
        }

        var preview = content.Length > 100 ? content.Substring(0, 100) + "..." : content;
        if (type == ClipboardContentType.Image) preview = "[Image]";
        else if (type == ClipboardContentType.FilePath) preview = "[File] " + preview;

        return new ClipboardItem
        {
            Content = content,
            ContentType = type,
            CreatedAt = DateTime.UtcNow.ToString("O"), // ISO 8601
            IsPinned = 0,
            PreviewText = preview
        };
    }
}
