using System;
using System.IO;

namespace Infrastructure.Sqlite;

public static class DbPathProvider
{
    public static string GetDatabasePath()
    {
        // exe хажууд /Data/pos.db  (таны зорилгод нийцүүлж)
        var baseDir = AppContext.BaseDirectory;
        var dataDir = Path.Combine(baseDir, "Data");
        Directory.CreateDirectory(dataDir);
        return Path.Combine(dataDir, "pos.db");
    }
}
