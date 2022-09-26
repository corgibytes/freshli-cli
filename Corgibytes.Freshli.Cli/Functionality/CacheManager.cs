using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheManager : ICacheManager
{
    private readonly IConfiguration _configuration;

    public CacheManager(IConfiguration configuration) => _configuration = configuration;

    public bool ValidateCacheDirectory()
    {
        var cacheDir = new DirectoryInfo(_configuration.CacheDir);
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

    public bool Prepare()
    {
        var cacheDir = new DirectoryInfo(_configuration.CacheDir);
        // Create the directory if it doesn't already exist
        if (!cacheDir.Exists)
        {
            cacheDir.Create();
        }
        else if (!ValidateCacheDirectory())
        {
            throw new CacheException(CliOutput.Exception_Cache_Prepare_NonEmpty);
        }

        using var db = new CacheContext(_configuration.CacheDir);
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

    public DirectoryInfo GetDirectoryInCache(string[] directoryStructure)
    {
        var cacheDir = new DirectoryInfo(_configuration.CacheDir);
        Prepare();
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

    public bool Destroy()
    {
        var cacheDir = new DirectoryInfo(_configuration.CacheDir);
        // If the directory doesn't exist, do nothing (be idempotent).
        if (!cacheDir.Exists)
        {
            throw new CacheException(CliOutput.Warning_Cache_Destroy_DoesNotExist) { IsWarning = true };
        }

        if (!ValidateCacheDirectory())
        {
            throw new CacheException(CliOutput.Exception_Cache_Destroy_NonCache);
        }

        cacheDir.Delete(true);
        return true;
    }

    public ICacheDb GetCacheDb() => new CacheDb(_configuration.CacheDir);

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
