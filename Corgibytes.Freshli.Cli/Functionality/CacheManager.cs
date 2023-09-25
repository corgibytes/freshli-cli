using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Resources;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NeoSmart.AsyncLock;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheManager : ICacheManager, IDisposable, IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly AsyncLock _cacheDbLock = new();
    private CacheDb? _cacheDb;

    public CacheManager(IConfiguration configuration) => _configuration = configuration;

    public async ValueTask<bool> ValidateCacheDirectory()
    {
        // use Task.Run to wait for file IO operation to complete
        var cacheDir = await Task.Run(() => new DirectoryInfo(_configuration.CacheDir));
        if (!cacheDir.Exists)
        {
            return false;
        }

        // using Task.Run to wait for file IO to complete
        var files = await Task.Run(() => cacheDir.GetFiles().Select(file => file.Name).ToList());
        // using Task.RUn to wait for file IO to complete
        var subDirectories = await Task.Run(() => cacheDir.GetDirectories());
        // Folder is valid cache if empty or if contains "freshli.db"
        return
            (!files.Any() && !subDirectories.Any())
            || files.Contains(CacheContext.CacheDbName);
    }

    public async ValueTask<bool> Prepare()
    {
        // use Task.Run to wait for file IO to complete
        var cacheDir = await Task.Run(() => new DirectoryInfo(_configuration.CacheDir));
        if (!cacheDir.Exists)
        {
            cacheDir.Create();
        }
        else if (!await ValidateCacheDirectory())
        {
            throw new CacheException(CliOutput.Exception_Cache_Prepare_NonEmpty);
        }

        await using var db = new CacheContext(_configuration.CacheDir);
        try
        {
            await MigrateIfPending(db);
        }
        catch (SqliteException e)
        {
            throw new CacheException(e.Message, e);
        }

        return true;
    }

    public async ValueTask<DirectoryInfo> GetDirectoryInCache(params string[] directoryStructure)
    {
        var cacheDir = new DirectoryInfo(_configuration.CacheDir);
        await Prepare();
        var focus = cacheDir;

        foreach (var directory in directoryStructure)
        {
            var found = false;
            // using Task.Run to wait for file IO
            var focusDirectories = await Task.Run(() => focus.GetDirectories(directory));
            foreach (var match in focusDirectories)
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
                // use Task.Run to wait for file IO
                focus = await Task.Run(() => focus.CreateSubdirectory(directory));
            }
        }

        return focus;
    }

    private static string GetFilePathHash(string filePath)
    {
        var sha256 = SHA256.Create();
        var sourceManifestHash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(filePath));
        return BitConverter.ToString(sourceManifestHash).Replace("-", string.Empty);
    }

    public async ValueTask<string> StoreBomInCache(string bomFilePath, Guid analysisId, DateTimeOffset asOfDateTime, string sourceManifestFilePath)
    {
        var sourceManifestFilePathHash = GetFilePathHash(sourceManifestFilePath);
        var targetFileName = $"{sourceManifestFilePathHash}-bom.json";

        var bomCacheDirInfo = await GetDirectoryInCache("boms", analysisId.ToString(), asOfDateTime.UtcDateTime.ToString("yyyyMMddTHHmmssZ"));
        var cachedBomFilePath = Path.Combine(bomCacheDirInfo.FullName, targetFileName);

        // use Task.Run to wait for file IO to complete
        await Task.Run(() => File.Copy(bomFilePath, cachedBomFilePath, true));

        return cachedBomFilePath;
    }

    public async ValueTask<bool> Destroy()
    {
        // use Task.Run to wait for file IO
        var cacheDir = await Task.Run(() => new DirectoryInfo(_configuration.CacheDir));
        // If the directory doesn't exist, do nothing (be idempotent).
        if (!cacheDir.Exists)
        {
            throw new CacheException(CliOutput.Warning_Cache_Destroy_DoesNotExist) { IsWarning = true };
        }

        if (!await ValidateCacheDirectory())
        {
            throw new CacheException(CliOutput.Exception_Cache_Destroy_NonCache);
        }

        // use Task.Run to wait for file IO
        await Task.Run(() => cacheDir.Delete(true));
        return true;
    }

    public async ValueTask<ICacheDb> GetCacheDb()
    {
        using (await _cacheDbLock.LockAsync())
        {
            if (_cacheDb != null)
            {
                return _cacheDb;
            }

            await Prepare();
            return new CacheDb(_configuration.CacheDir);
        }
    }

    private static async ValueTask MigrateIfPending(CacheContext context)
    {
        var pending = await context.Database.GetPendingMigrationsAsync();
        if (!pending.Any())
        {
            return;
        }

        await context.Database.MigrateAsync();
    }

    public void Dispose()
    {
        using (_cacheDbLock.Lock())
        {
            _cacheDb?.Dispose();
            _cacheDb = null;
        }

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        using (await _cacheDbLock.LockAsync())
        {
            if (_cacheDb != null)
            {
                await _cacheDb.DisposeAsync();
                _cacheDb = null;
            }
        }

        GC.SuppressFinalize(this);
    }
}
