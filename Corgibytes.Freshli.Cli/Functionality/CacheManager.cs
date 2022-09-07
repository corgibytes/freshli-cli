using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheManager : ICacheManager
{
    public bool ValidateDirIsCache(string cacheDirPath)
    {
        var cacheDir = new DirectoryInfo(cacheDirPath);
        if (!cacheDir.Exists)
        {
            return false;
        }
        var dirContents = cacheDir.GetFiles().Select(file => file.Name).ToList();
        // Folder is valid cache if empty or if contains "freshli.db"
        return
            (!dirContents.Any() && !cacheDir.GetDirectories().Any())
            || dirContents.Contains(CacheContext.CacheDbName);
    }

    public bool Prepare(string cacheDirPath)
    {
        var cacheDir = new DirectoryInfo(cacheDirPath);
        // Create the directory if it doesn't already exist
        if (!cacheDir.Exists)
        {
            cacheDir.Create();
        }
        else if (!ValidateDirIsCache(cacheDirPath))
        {
            throw new CacheException(CliOutput.Exception_Cache_Prepare_NonEmpty);
        }

        using var db = new CacheContext(cacheDirPath);
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

    public DirectoryInfo GetDirectoryInCache(string cacheDirPath, string[] directoryStructure)
    {
        var cacheDir = new DirectoryInfo(cacheDirPath);
        Prepare(cacheDirPath);
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

    public bool Destroy(string cacheDirPath)
    {
        var cacheDir = new DirectoryInfo(cacheDirPath);
        // If the directory doesn't exist, do nothing (be idempotent).
        if (!cacheDir.Exists)
        {
            throw new CacheException(CliOutput.Warning_Cache_Destroy_DoesNotExist) { IsWarning = true };
        }

        if (!ValidateDirIsCache(cacheDirPath))
        {
            throw new CacheException(CliOutput.Exception_Cache_Destroy_NonCache);
        }

        cacheDir.Delete(true);
        return true;
    }

    public ICacheDb GetCacheDb(string cacheDir) => new CacheDb(cacheDir);

    private static void MigrateIfPending(CacheContext context)
    {
        var pending = context.Database.GetPendingMigrations();
        if (!pending.Any())
        {
            return;
        }

        context.Database.Migrate();
    }
}
