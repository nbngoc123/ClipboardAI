using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClipboardAI.Models;

namespace ClipboardAI.Services.AI;

public interface IAIService
{
    Task<List<ExtractedField>> ExtractDataAsync(string text, CancellationToken cancellationToken = default);
    Task<List<ExtractedField>> SummarizeAndTranslateAsync(string text, CancellationToken cancellationToken = default);
}
