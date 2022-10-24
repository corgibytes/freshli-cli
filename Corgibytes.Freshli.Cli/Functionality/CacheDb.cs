using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheDb : ICacheDb, IDisposable
{
    private bool _disposed;

    public CacheDb(string cacheDir) => Db = new CacheContext(cacheDir);

    private CacheContext Db { get; }

    public Guid SaveAnalysis(CachedAnalysis analysis)
    {
        if (analysis.Id == Guid.Empty)
        {
            var savedEntity = Db.CachedAnalyses.Add(analysis);
            Db.SaveChanges();
            return savedEntity.Entity.Id;
        }

        Db.CachedAnalyses.Update(analysis);
        Db.SaveChanges();
        return analysis.Id;
    }

    public CachedAnalysis? RetrieveAnalysis(Guid id) => Db.CachedAnalyses.Find(id);
    public CachedGitSource? RetrieveCachedGitSource(CachedGitSourceId id) => Db.CachedGitSources.Find(id.Id);
    public void RemoveCachedGitSource(CachedGitSource cachedGitSource) => Db.CachedGitSources.Remove(cachedGitSource);

    public void AddCachedGitSource(CachedGitSource cachedGitSource)
    {
        Db.CachedGitSources.Add(cachedGitSource);
        Db.SaveChanges();
    }

    public CachedHistoryStopPoint? RetrieveHistoryStopPoint(int historyStopPointId) =>
        Db.CachedHistoryStopPoints.Find(historyStopPointId);

    public int AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint)
    {
        var savedEntity = Db.CachedHistoryStopPoints.Add(historyStopPoint);
        Db.SaveChanges();
        return savedEntity.Entity.Id;
    }

    public CachedPackageLibYear? RetrievePackageLibYear(int packageLibYearId) =>
        Db.CachedPackageLibYears.Find(packageLibYearId);

    public int AddPackageLibYear(CachedPackageLibYear packageLibYear)
    {
        var savedEntity = Db.CachedPackageLibYears.Add(packageLibYear);
        Db.SaveChanges();
        return savedEntity.Entity.Id;
    }

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
}
