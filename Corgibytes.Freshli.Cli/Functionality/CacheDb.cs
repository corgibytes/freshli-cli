using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;
using PackageUrl;

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

    public List<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl) => Db.CachedPackages
        .Where(value => value.PackageUrlWithoutVersion == packageUrl.ToString()).ToList();

    public void StoreCachedReleaseHistory(List<CachedPackage> packages)
    {
        Db.CachedPackages.AddRange(packages);
        Db.SaveChanges();
    }

    public List<string> RetrieveCachedManifests(int historyStopPointId, string agentExecutablePath) =>
        Db.CachedManifestPaths.Where(value =>
            value.HistoryStopPointId == historyStopPointId &&
            value.AgentExecutablePath == agentExecutablePath
        ).Select(value => value.ManifestPath).ToList();

    public void StoreCachedManifests(int historyStopPointId, string agentExecutablePath, List<string> manifestPaths)
    {
        var cachedManifestPaths = manifestPaths.Select(manifestPath => new CachedManifestPath
        {
            HistoryStopPointId = historyStopPointId,
            AgentExecutablePath = agentExecutablePath,
            ManifestPath = manifestPath
        })
            .ToList();

        if (manifestPaths.Count <= 0)
        {
            return;
        }

        Db.CachedManifestPaths.AddRange(cachedManifestPaths);
        Db.SaveChanges();
    }

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
