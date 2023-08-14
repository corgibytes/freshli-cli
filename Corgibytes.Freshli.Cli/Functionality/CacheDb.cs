using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PackageUrl;
using Polly;
using Polly.Retry;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheDb : ICacheDb
{
    private readonly ConcurrentDictionary<Guid, CachedAnalysis> _analysisMemoryCache = new();
    private readonly ConcurrentDictionary<string, CachedGitSource> _gitSourceMemoryCache = new();
    private readonly ConcurrentDictionary<int, CachedHistoryStopPoint> _historyStopPointMemoryCache = new();
    private readonly ConcurrentDictionary<int, CachedPackageLibYear> _packageLibYearMemoryCache = new();
    private readonly ConcurrentDictionary<string, IList<CachedPackage>> _releaseHistoryMemoryCache = new();

    private readonly string _cacheDir;

    private readonly AsyncRetryPolicy _sqliteRetryPolicy = Policy
        .Handle<SqliteException>()
        .WaitAndRetryAsync(6, retryAttempt =>
            TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0))
        );


    public CacheDb(string cacheDir) => _cacheDir = cacheDir;

    public async ValueTask<Guid> SaveAnalysis(CachedAnalysis analysis)
    {
        await using var context = new CacheContext(_cacheDir);
        await using var transaction = await context.Database.BeginTransactionAsync();

        if (analysis.Id == Guid.Empty)
        {
            var savedEntity = await context.CachedAnalyses.AddAsync(analysis);
            await SaveChanges(context);
            await transaction.CommitAsync();
            return savedEntity.Entity.Id;
        }

        context.CachedAnalyses.Update(analysis);
        await SaveChanges(context);
        await transaction.CommitAsync();

        _analysisMemoryCache.TryAdd(analysis.Id, analysis);
        return analysis.Id;
    }

    public async ValueTask<CachedAnalysis?> RetrieveAnalysis(Guid id)
    {
        if (_analysisMemoryCache.TryGetValue(id, out var value))
        {
            return await ValueTask.FromResult(value);
        }

        await using var context = new CacheContext(_cacheDir);
        value = await context.CachedAnalyses.FindAsync(id);
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

        await using var context = new CacheContext(_cacheDir);
        value = await _sqliteRetryPolicy.ExecuteAsync(async () => await context.CachedGitSources.FindAsync(id.Id));
        if (value != null)
        {
            _gitSourceMemoryCache.TryAdd(id.Id, value);
        }

        return value;
    }

    public async ValueTask RemoveCachedGitSource(CachedGitSource cachedGitSource)
    {
        _gitSourceMemoryCache.TryRemove(cachedGitSource.Id, out _);

        await using var context = new CacheContext(_cacheDir);
        context.CachedGitSources.Remove(cachedGitSource);
        await SaveChanges(context);
    }

    public async ValueTask AddCachedGitSource(CachedGitSource cachedGitSource)
    {
        _gitSourceMemoryCache.TryAdd(cachedGitSource.Id, cachedGitSource);

        await using var context = new CacheContext(_cacheDir);
        await context.CachedGitSources.AddAsync(cachedGitSource);
        await SaveChanges(context);
    }

    public async ValueTask<CachedHistoryStopPoint?> RetrieveHistoryStopPoint(int historyStopPointId)
    {
        if (_historyStopPointMemoryCache.TryGetValue(historyStopPointId, out var value))
        {
            return await ValueTask.FromResult(value);
        }

        await using var context = new CacheContext(_cacheDir);
        value = await _sqliteRetryPolicy.ExecuteAsync(
            async () => await context.CachedHistoryStopPoints.FindAsync(historyStopPointId));
        if (value != null)
        {
            _historyStopPointMemoryCache.TryAdd(historyStopPointId, value);
        }

        return value;
    }

    public async ValueTask<int> AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint)
    {
        await using var context = new CacheContext(_cacheDir);
        var savedEntity = await context.CachedHistoryStopPoints.AddAsync(historyStopPoint);
        await SaveChanges(context);
        _historyStopPointMemoryCache.TryAdd(savedEntity.Entity.Id, savedEntity.Entity);
        return savedEntity.Entity.Id;
    }

    public async ValueTask<CachedPackageLibYear?> RetrievePackageLibYear(int packageLibYearId)
    {
        if (_packageLibYearMemoryCache.TryGetValue(packageLibYearId, out var value))
        {
            return await ValueTask.FromResult(value);
        }

        await using var context = new CacheContext(_cacheDir);
        value = await _sqliteRetryPolicy.ExecuteAsync(
            async () => await context.CachedPackageLibYears.FindAsync(packageLibYearId));
        if (value != null)
        {
            _packageLibYearMemoryCache.TryAdd(packageLibYearId, value);
        }
        return value;
    }

    public async IAsyncEnumerable<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl)
    {
        await using var context = new CacheContext(_cacheDir);
        IAsyncEnumerable<CachedPackage> query;

        if (_releaseHistoryMemoryCache.TryGetValue(packageUrl.ToString()!, out var cacheList))
        {
            query = cacheList.ToAsyncEnumerable();
        }
        else
        {
            query = context.CachedPackages
                .Where(value => value.PackageUrlWithoutVersion == packageUrl.FormatWithoutVersion())
                .AsAsyncEnumerable();
        }

        var list = new List<CachedPackage>();

        await using var enumerator = query.GetAsyncEnumerator();
        while (await _sqliteRetryPolicy.ExecuteAsync(async () => await enumerator.MoveNextAsync()))
        {
            list.Add(enumerator.Current);
            yield return enumerator.Current;
        }

        _releaseHistoryMemoryCache.TryAdd(packageUrl.ToString()!, list);
    }

    public async ValueTask StoreCachedReleaseHistory(List<CachedPackage> packages)
    {
        await using var context = new CacheContext(_cacheDir);
        await context.CachedPackages.AddRangeAsync(packages);
        var firstPackage = packages.First();
        _releaseHistoryMemoryCache.TryAdd(firstPackage.PackageUrlWithoutVersion, packages);
        await SaveChanges(context);
    }

    public async ValueTask<int> AddPackageLibYear(CachedPackageLibYear packageLibYear)
    {
        await using var context = new CacheContext(_cacheDir);
        var savedEntity = await context.CachedPackageLibYears.AddAsync(packageLibYear);
        await SaveChanges(context);
        _packageLibYearMemoryCache.TryAdd(savedEntity.Entity.Id, savedEntity.Entity);
        return savedEntity.Entity.Id;
    }

    private async ValueTask SaveChanges(DbContext context)
    {
        await _sqliteRetryPolicy.ExecuteAsync(async () => await context.SaveChangesAsync());
    }
}
