using System;
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
        return analysis.Id;
    }

    public async ValueTask<CachedAnalysis?> RetrieveAnalysis(Guid id) => await Db.CachedAnalyses.FindAsync(id);

    public async ValueTask<CachedGitSource?> RetrieveCachedGitSource(CachedGitSourceId id) =>
        await Db.CachedGitSources.FindAsync(id.Id);

    public async ValueTask RemoveCachedGitSource(CachedGitSource cachedGitSource)
    {
        Db.CachedGitSources.Remove(cachedGitSource);
        await SaveChanges();
    }

    public async ValueTask AddCachedGitSource(CachedGitSource cachedGitSource)
    {
        await Db.CachedGitSources.AddAsync(cachedGitSource);
        await SaveChanges();
    }

    public async ValueTask<CachedHistoryStopPoint?> RetrieveHistoryStopPoint(int historyStopPointId) =>
        await Db.CachedHistoryStopPoints.FindAsync(historyStopPointId);

    public async ValueTask<int> AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint)
    {
        var savedEntity = await Db.CachedHistoryStopPoints.AddAsync(historyStopPoint);
        await SaveChanges();
        return savedEntity.Entity.Id;
    }

    public async ValueTask<CachedPackageLibYear?> RetrievePackageLibYear(int packageLibYearId) =>
        await Db.CachedPackageLibYears.FindAsync(packageLibYearId);

    public IAsyncEnumerable<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl) =>
        Db.CachedPackages.Where(value => value.PackageUrlWithoutVersion == packageUrl.ToString()).AsAsyncEnumerable();

    public async ValueTask StoreCachedReleaseHistory(List<CachedPackage> packages)
    {
        await Db.CachedPackages.AddRangeAsync(packages);
        await SaveChanges();
    }

    public async ValueTask<int> AddPackageLibYear(CachedPackageLibYear packageLibYear)
    {
        var savedEntity = await Db.CachedPackageLibYears.AddAsync(packageLibYear);
        await SaveChanges();
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
