using System.Threading.Tasks;
using Dapper;

namespace ClipboardAI.Data;

public class DatabaseInitializer
{
    private readonly DatabaseContext _context;

    public DatabaseInitializer(DatabaseContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        using var conn = _context.CreateConnection();
        
        // Cấu hình PRAGMA cho SQLite
        await conn.ExecuteAsync("PRAGMA journal_mode = WAL;");
        await conn.ExecuteAsync("PRAGMA synchronous = NORMAL;");

        // Tạo bảng ClipboardItems
        var createClipboardItemsTable = @"
            CREATE TABLE IF NOT EXISTS ClipboardItems (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                Content     TEXT    NOT NULL,
                ContentType INTEGER NOT NULL DEFAULT 0,
                CreatedAt   TEXT    NOT NULL,
                IsPinned    INTEGER NOT NULL DEFAULT 0,
                PreviewText TEXT,
                Tags        TEXT
            );";
        await conn.ExecuteAsync(createClipboardItemsTable);

        // Tạo bảng AppSettings
        var createAppSettingsTable = @"
            CREATE TABLE IF NOT EXISTS AppSettings (
                Key   TEXT PRIMARY KEY,
                Value TEXT
            );";
        await conn.ExecuteAsync(createAppSettingsTable);

        // Tạo indexes
        await conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_createdat ON ClipboardItems(CreatedAt DESC);");
        await conn.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_pinned ON ClipboardItems(IsPinned DESC);");
    }
}
