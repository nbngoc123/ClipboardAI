using System;
using System.Security.Cryptography;
using System.Text;

namespace ClipboardAI.Services.Clipboard;

public static class ClipboardHasher
{
    public static string ComputeHash(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
        
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}
