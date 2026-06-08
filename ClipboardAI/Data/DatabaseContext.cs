using System.Data;
using Microsoft.Data.Sqlite;
using ClipboardAI.Helpers;

namespace ClipboardAI.Data;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext()
    {
        AppPaths.EnsureDirectoriesCreated();
        _connectionString = $"Data Source={AppPaths.DatabasePath}";
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
