using System;

namespace ClipboardAI.Models;

public class ClipboardItem
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public ClipboardContentType ContentType { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public int IsPinned { get; set; }
    public string? PreviewText { get; set; }
    public string? Tags { get; set; }

    public string DisplayCreatedAt
    {
        get
        {
            if (DateTime.TryParse(CreatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime dt))
            {
                return dt.ToLocalTime().ToString("MMM dd, yyyy - hh:mm tt");
            }
            return CreatedAt;
        }
    }
}
