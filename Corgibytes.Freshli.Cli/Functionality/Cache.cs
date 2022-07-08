using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

[Serializable]
public class CacheException : Exception
{
    public CacheException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CacheException(string message) : base(message)
    {
    }

    protected CacheException(SerializationInfo info, StreamingContext context) : base(info, context) =>
        IsWarning = info.GetBoolean("IsWarning");

    public bool IsWarning { get; init; }

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
        var dirContents = cacheDir.GetFiles().Select(file => file.Name).ToList();
        // Folder is valid cache if empty or if contains "freshli.db"
        return
            (!dirContents.Any() && !cacheDir.GetDirectories().Any())
            || dirContents.Contains(CacheContext.CacheDbName);
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
            throw new CacheException("We cannot use an existing non-empty directory as a cache directory.");
        }

        using var db = new CacheContext(cacheDir);
        try
        {
            MigrateIfPending(db);
        }
        catch (SqliteException e)
        {
            throw new CacheException(e.Message, e);
        }

        return true;
    }

    public static DirectoryInfo GetDirectoryInCache(DirectoryInfo cacheDir, string[] directoryStructure)
    {
        Prepare(cacheDir);
        var focus = cacheDir;

        foreach (var directory in directoryStructure)
        {
            var found = false;
            foreach (var match in focus.GetDirectories(directory))
            {
                if (match.Name == directory)
                {
                    focus = match;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                focus = focus.CreateSubdirectory(directory);
            }
        }

        return focus;
    }

    public static bool Destroy(DirectoryInfo cacheDir)
    {
        // If the directory doesn't exist, do nothing (be idempotent).
        if (!cacheDir.Exists)
        {
            throw new CacheException("Cache directory already destroyed or does not exist.") { IsWarning = true };
        }

        if (!ValidateDirIsCache(cacheDir))
        {
            throw new CacheException("This directory is not a Freshli cache. Directory not destroyed.");
        }

        cacheDir.Delete(true);
        return true;
    }
}
