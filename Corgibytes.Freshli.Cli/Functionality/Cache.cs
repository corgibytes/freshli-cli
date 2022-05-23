using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

[Serializable]
public class CacheException : Exception
{
    public bool IsWarning { get; init; }

    public CacheException(string message, Exception innerException) : base(message, innerException) { }
    public CacheException(string message) : base(message) { }

    protected CacheException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        IsWarning = info.GetBoolean("IsWarning");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        info.AddValue("IsWarning", IsWarning);

        base.GetObjectData(info, context);
    }
}

public static class Cache
{
    private static void MigrateIfPending(CacheContext context)
    {
        var pending = context.Database.GetPendingMigrations();
        if (!pending.Any())
        {
            return;
        }

        context.Database.Migrate();
    }

    private static bool ValidateDirIsCache(DirectoryInfo cacheDir)
    {
        List<string> dirContents = cacheDir.GetFiles().Select(file => file.Name).ToList();
        // Folder is valid cache if empty or if contains "freshli.db"
        return (!dirContents.Any() || dirContents.Contains(CacheContext.CacheDbName));
    }

    public static bool Prepare(DirectoryInfo cacheDir)
    {
        Console.Out.WriteLine($"Preparing cache at {cacheDir}");

        // Create the directory if it doesn't already exist
        if (!cacheDir.Exists)
        {
            cacheDir.Create();
        }
        else if (!ValidateDirIsCache(cacheDir))
        {
            throw new CacheException($"{CliOutput.Exception_Cache_Prepare_NonEmpty}");
        }

        using var db = new CacheContext(cacheDir);
        try
        {
            MigrateIfPending(db);
        }
        catch (Microsoft.Data.Sqlite.SqliteException e)
        {
            throw new CacheException(e.Message, e);
        }

        return true;
    }

    public static bool Destroy(DirectoryInfo cacheDir)
    {
        // If the directory doesn't exist, do nothing (be idempotent).
        if (!cacheDir.Exists)
        {
            throw new CacheException($"{CliOutput.Warning_Cache_Destroy_DoesNotExist}") { IsWarning = true };
        }

        if (!ValidateDirIsCache(cacheDir))
        {
            throw new CacheException($"{CliOutput.Exception_Cache_Destroy_NonCache}");
        }

        cacheDir.Delete(true);
        return true;
    }
}
