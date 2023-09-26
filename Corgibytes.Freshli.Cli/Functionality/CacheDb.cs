using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NeoSmart.AsyncLock;
using PackageUrl;
using Polly;
using Polly.Retry;

namespace Corgibytes.Freshli.Cli.Functionality;

// TODO: This class should handle validating that the data is correct before saving it.
public class CacheDb : ICacheDb, IDisposable, IAsyncDisposable
{
    private readonly AsyncLock _cacheDbLock = new();

    private readonly AsyncRetryPolicy _sqliteRetryPolicy = Policy
        .Handle<SqliteException>()
        .WaitAndRetryAsync(6, retryAttempt =>
            TimeSpan.FromMilliseconds(Math.Pow(10, retryAttempt / 2.0))
        );

    private readonly CacheContext _context;

    public CacheDb(string cacheDir)
    {
        _context = new CacheContext(cacheDir);
    }

    public async ValueTask<Guid> SaveAnalysis(CachedAnalysis analysis)
    {
        using (await _cacheDbLock.LockAsync())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (analysis.Id == Guid.Empty)
            {
                var savedEntity = await _context.CachedAnalyses.AddAsync(analysis);
                await SaveChanges(_context);
                await transaction.CommitAsync();
                return savedEntity.Entity.Id;
            }

            _context.CachedAnalyses.Update(analysis);
            await SaveChanges(_context);
            await transaction.CommitAsync();

            return analysis.Id;
        }
    }

    public async ValueTask<CachedAnalysis?> RetrieveAnalysis(Guid id)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var value = await _context.CachedAnalyses.FindAsync(id);

            return value;
        }
    }

    public async ValueTask<CachedGitSource?> RetrieveCachedGitSource(CachedGitSourceId id)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var value = await _sqliteRetryPolicy.ExecuteAsync(async () =>
                await _context.CachedGitSources.FindAsync(id.Id));

            return value;
        }
    }

    public async ValueTask RemoveCachedGitSource(CachedGitSource cachedGitSource)
    {
        using (await _cacheDbLock.LockAsync())
        {
            _context.CachedGitSources.Remove(cachedGitSource);
            await SaveChanges(_context);
        }
    }

    public async ValueTask<CachedGitSource> AddCachedGitSource(CachedGitSource cachedGitSource)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var savedEntity = await _context.CachedGitSources.AddAsync(cachedGitSource);
            await SaveChanges(_context);
            return savedEntity.Entity;
        }
    }

    public async ValueTask<CachedManifest> AddManifest(CachedHistoryStopPoint historyStopPoint, string manifestFilePath)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var retrievedHistoryStopPoint = await _context.CachedHistoryStopPoints.FindAsync(historyStopPoint.Id);
            var manifest = new CachedManifest
            {
                HistoryStopPoint = retrievedHistoryStopPoint!,
                ManifestFilePath = manifestFilePath
            };

            var savedManifest = _context.CachedManifests.Add(manifest);
            await SaveChanges(_context);
            return savedManifest.Entity;
        }
    }

    public async ValueTask<CachedManifest?> RetrieveManifest(CachedHistoryStopPoint historyStopPoint, string manifestFilePath)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var value = await _sqliteRetryPolicy.ExecuteAsync(
                async () => await _context.CachedManifests.FirstOrDefaultAsync(entry =>
                    entry.HistoryStopPoint.Id == historyStopPoint.Id && entry.ManifestFilePath == manifestFilePath
                )
            );

            return value;
        }
    }

    public async ValueTask<CachedHistoryStopPoint?> RetrieveHistoryStopPoint(int historyStopPointId)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var value = await _sqliteRetryPolicy.ExecuteAsync(
                async () => await _context.CachedHistoryStopPoints.FindAsync(historyStopPointId));

            return value;
        }
    }

    public async ValueTask<CachedHistoryStopPoint> AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var savedEntity = await _context.CachedHistoryStopPoints.AddAsync(historyStopPoint);
            await SaveChanges(_context);
            return savedEntity.Entity;
        }
    }

    public async ValueTask<CachedPackageLibYear?> RetrievePackageLibYear(PackageURL packageUrl, DateTimeOffset asOfDateTime)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var rawPackageUrl = packageUrl.ToString()!;

            var value = await _sqliteRetryPolicy.ExecuteAsync(
                async () => await _context.CachedPackageLibYears.FirstOrDefaultAsync(entry =>
                    entry.PackageUrl == rawPackageUrl && entry.AsOfDateTime == asOfDateTime
                )
            );

            return value;
        }
    }

    public async IAsyncEnumerable<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl)
    {
        using (await _cacheDbLock.LockAsync())
        {
            var query = _context.CachedPackages
                .Where(value => value.PackageUrlWithoutVersion == packageUrl.FormatWithoutVersion())
                .AsAsyncEnumerable();

            await using var enumerator = query.GetAsyncEnumerator();
            while (await _sqliteRetryPolicy.ExecuteAsync(async () => await enumerator.MoveNextAsync()))
            {
                yield return enumerator.Current;
            }
        }
    }

    public async ValueTask StoreCachedReleaseHistory(List<CachedPackage> packages)
    {
        using (await _cacheDbLock.LockAsync())
        {
            await _context.CachedPackages.AddRangeAsync(packages);

            await SaveChanges(_context);
        }
    }

    public async ValueTask<CachedPackageLibYear> AddPackageLibYear(CachedManifest manifest, CachedPackageLibYear packageLibYear)
    {
        using (await _cacheDbLock.LockAsync())
        {
            if (manifest.Id == 0)
            {
                // ReSharper disable once LocalizableElement
                throw new ArgumentException($"{nameof(manifest)} must be saved before adding a PackageLibYear",
                    nameof(manifest));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var manifestEntity = await _context.CachedManifests
                .Include(value => value.HistoryStopPoint)
                .FirstAsync(value => value.Id == manifest.Id);

            if (manifestEntity.HistoryStopPoint.AsOfDateTime != packageLibYear.AsOfDateTime)
            {
                throw new ArgumentException(
                    $"{nameof(packageLibYear)}'s {nameof(packageLibYear.AsOfDateTime)} must match {nameof(manifest.HistoryStopPoint)}'s {nameof(manifest.HistoryStopPoint.AsOfDateTime)}"
                );
            }

            CachedPackageLibYear savedPackageLibYearEntity;
            if (packageLibYear.Id != 0)
            {
                savedPackageLibYearEntity = (await _context.CachedPackageLibYears.FindAsync(packageLibYear.Id))!;
                if (!_context.CachedPackageLibYears.Any(value => value.Manifests.Select(point => point.Id).Contains(manifest.Id)))
                {
                    savedPackageLibYearEntity.Manifests.Add(manifestEntity);
                }
            }
            else
            {
                packageLibYear.Manifests.Add(manifestEntity);
                savedPackageLibYearEntity = (await _context.CachedPackageLibYears.AddAsync(packageLibYear)).Entity;
            }

            await SaveChanges(_context);

            await transaction.CommitAsync();

            return savedPackageLibYearEntity;
        }
    }

    private async ValueTask SaveChanges(DbContext context)
    {
        await _sqliteRetryPolicy.ExecuteAsync(async () =>
            await context.SaveChangesAsync()
        );
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
