using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using ClipboardAI.Models;

namespace ClipboardAI.Data.Repositories;

public class ClipboardRepository : IClipboardRepository
{
    private readonly DatabaseContext _context;

    public ClipboardRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClipboardItem>> GetRecentAsync(int limit = 50)
    {
        using var conn = _context.CreateConnection();
        return await conn.QueryAsync<ClipboardItem>(
            "SELECT * FROM ClipboardItems ORDER BY IsPinned DESC, CreatedAt DESC LIMIT @limit",
            new { limit }
        );
    }

    public async Task<int> InsertAsync(ClipboardItem item)
    {
        using var conn = _context.CreateConnection();
        var sql = @"
            INSERT INTO ClipboardItems (Content, ContentType, CreatedAt, IsPinned, PreviewText, Tags)
            VALUES (@Content, @ContentType, @CreatedAt, @IsPinned, @PreviewText, @Tags);
            SELECT last_insert_rowid();";
        
        var id = await conn.ExecuteScalarAsync<int>(sql, item);
        item.Id = id;
        return id;
    }

    public async Task DeleteOldestAsync(int keepCount)
    {
        using var conn = _context.CreateConnection();
        var sql = @"
            DELETE FROM ClipboardItems 
            WHERE IsPinned = 0 
            AND Id NOT IN (
                SELECT Id FROM ClipboardItems 
                WHERE IsPinned = 0 
                ORDER BY CreatedAt DESC 
                LIMIT @keepCount
            );";
        await conn.ExecuteAsync(sql, new { keepCount });
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _context.CreateConnection();
        var sql = "DELETE FROM ClipboardItems WHERE Id = @id;";
        await conn.ExecuteAsync(sql, new { id });
    }
}
