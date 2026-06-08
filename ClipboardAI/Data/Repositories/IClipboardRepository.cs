using System.Collections.Generic;
using System.Threading.Tasks;
using ClipboardAI.Models;

namespace ClipboardAI.Data.Repositories;

public interface IClipboardRepository
{
    Task<IEnumerable<ClipboardItem>> GetRecentAsync(int limit = 50);
    Task<int> InsertAsync(ClipboardItem item);
    Task DeleteOldestAsync(int keepCount);
    Task DeleteAsync(int id);
}
