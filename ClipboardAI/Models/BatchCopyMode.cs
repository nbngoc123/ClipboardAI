using System.Collections.Generic;

namespace ClipboardAI.Models;

public class BatchCopyMode
{
    public bool IsRecording { get; set; } = false;
    public Queue<ClipboardItem> Queue { get; set; } = new();
}
