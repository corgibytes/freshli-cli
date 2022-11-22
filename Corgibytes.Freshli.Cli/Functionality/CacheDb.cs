using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PackageUrl;
using Polly;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheDb : ICacheDb, IDisposable
{
    private bool _disposed;

    private readonly ConcurrentDictionary<Guid, CachedAnalysis> _analysisMemoryCache = new();
    private readonly ConcurrentDictionary<string, CachedGitSource> _gitSourceMemoryCache = new();
    private readonly ConcurrentDictionary<int, CachedHistoryStopPoint> _historyStopPointMemoryCache = new();
    private readonly ConcurrentDictionary<int, CachedPackageLibYear> _packageLibYearMemoryCache = new();
    private readonly ConcurrentDictionary<string, IList<CachedPackage>> _releaseHistoryMemoryCache = new();

    public CacheDb(string cacheDir) => Db = new CacheContext(cacheDir);

    private CacheContext Db { get; }

    public async ValueTask<Guid> SaveAnalysis(CachedAnalysis analysis)
    {
        if (analysis.Id == Guid.Empty)
        {
            var savedEntity = await Db.CachedAnalyses.AddAsync(analysis);
            await SaveChanges();
            return savedEntity.Entity.Id;
        }

        Db.CachedAnalyses.Update(analysis);
        await SaveChanges();

        _analysisMemoryCache[analysis.Id] = analysis;
        return analysis.Id;
    }

    public async ValueTask<CachedAnalysis?> RetrieveAnalysis(Guid id)
    {
        if (_analysisMemoryCache.TryGetValue(id, out var value))
        {
            return await ValueTask.FromResult(value);
        }

        value = await Db.CachedAnalyses.FindAsync(id);
        if (value != null)
        {
            _analysisMemoryCache.TryAdd(id, value);
        }

        return value;
    }

    public async ValueTask<CachedGitSource?> RetrieveCachedGitSource(CachedGitSourceId id)
    {
        if (_gitSourceMemoryCache.TryGetValue(id.Id, out var value))
        {
            return await ValueTask.FromResult(value);
        }

        value = await Db.CachedGitSources.FindAsync(id.Id);
        if (value != null)
        {
            _gitSourceMemoryCache.TryAdd(id.Id, value);
        }

        return value;
    }

    public async ValueTask RemoveCachedGitSource(CachedGitSource cachedGitSource)
    {
        _gitSourceMemoryCache.TryRemove(cachedGitSource.Id, out _);
        Db.CachedGitSources.Remove(cachedGitSource);
        await SaveChanges();
    }

    public async ValueTask AddCachedGitSource(CachedGitSource cachedGitSource)
    {
        _gitSourceMemoryCache[cachedGitSource.Id] = cachedGitSource;
        await Db.CachedGitSources.AddAsync(cachedGitSource);
        await SaveChanges();
    }

    public async ValueTask<CachedHistoryStopPoint?> RetrieveHistoryStopPoint(int historyStopPointId)
    {
        if (_historyStopPointMemoryCache.TryGetValue(historyStopPointId, out var value))
        {
            return await ValueTask.FromResult(value);
        }

        value = await Db.CachedHistoryStopPoints.FindAsync(historyStopPointId);
        if (value != null)
        {
            _historyStopPointMemoryCache.TryAdd(historyStopPointId, value);
        }

        return value;
    }

    public async ValueTask<int> AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint)
    {
        var savedEntity = await Db.CachedHistoryStopPoints.AddAsync(historyStopPoint);
        await SaveChanges();
        _historyStopPointMemoryCache[savedEntity.Entity.Id] = savedEntity.Entity;
        return savedEntity.Entity.Id;
    }

    public async ValueTask<CachedPackageLibYear?> RetrievePackageLibYear(int packageLibYearId)
    {
        if (_packageLibYearMemoryCache.TryGetValue(packageLibYearId, out var value))
        {
            return await ValueTask.FromResult(value);
        }

        value = await Db.CachedPackageLibYears.FindAsync(packageLibYearId);
        if (value != null)
        {
            _packageLibYearMemoryCache.TryAdd(packageLibYearId, value);
        }
        return value;
    }

    public async IAsyncEnumerable<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl)
    {
        IAsyncEnumerable<CachedPackage> query;
        if (_releaseHistoryMemoryCache.TryGetValue(packageUrl.ToString(), out var cacheList))
        {
            query = cacheList.ToAsyncEnumerable();
        }
        else
        {
            query = Db.CachedPackages.Where(value => value.PackageUrlWithoutVersion == packageUrl.ToString())
                .AsAsyncEnumerable();
        }

        var list = new List<CachedPackage>();
        await foreach (var package in query)
        {
            yield return package;
        }

        _releaseHistoryMemoryCache[packageUrl.ToString()] = list;
    }

    public async ValueTask StoreCachedReleaseHistory(List<CachedPackage> packages)
    {
        await Db.CachedPackages.AddRangeAsync(packages);
        var firstPackage = packages.First();
        _releaseHistoryMemoryCache[firstPackage.PackageUrlWithoutVersion] = packages;
        await SaveChanges();
    }

    public async ValueTask<int> AddPackageLibYear(CachedPackageLibYear packageLibYear)
    {
        var savedEntity = await Db.CachedPackageLibYears.AddAsync(packageLibYear);
        await SaveChanges();
        _packageLibYearMemoryCache[savedEntity.Entity.Id] = savedEntity.Entity;
        return savedEntity.Entity.Id;
    }

    // TODO: Should this call DisposeAsync?
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Db.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private async ValueTask SaveChanges()
    {
        await Policy
            .Handle<SqliteException>()
            .WaitAndRetryAsync(6, retryAttempt =>
                TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0))
            )
            .ExecuteAsync(async () => await Db.SaveChangesAsync());
    }
}
