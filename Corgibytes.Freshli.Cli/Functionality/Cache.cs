using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

public static class Cache
{
    private static void MigrateIfPending(CacheContext context)
    {
        var pending = context.Database.GetPendingMigrations();
        if (pending.Any() == false)
        {
            return;
        }

        context.Database.Migrate();
    }
    public static bool Prepare(DirectoryInfo cacheDir)
    {
        Console.Out.WriteLine($"Preparing cache at {cacheDir}");

        // Create the directory if it doesn't already exist
        if (cacheDir.Exists == false)
        {
            cacheDir.Create();
        }

        using var db = new CacheContext(cacheDir);
        try
        {
            MigrateIfPending(db);
        }
        catch (Microsoft.Data.Sqlite.SqliteException e)
        {
            Console.Out.WriteLine(e.Message);
            return false;
        }

        return true;
    }
}
