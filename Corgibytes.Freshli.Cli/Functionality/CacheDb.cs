using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
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
        var savedEntity = Db.CachedAnalyses.Add(analysis);
        Db.SaveChanges();
        return savedEntity.Entity.Id;
    }

    public CachedAnalysis? RetrieveAnalysis(Guid id) => Db.CachedAnalyses.Find(id);
    public CachedGitSource? RetrieveCachedGitSource(CachedGitSourceId id) => Db.CachedGitSources.Find(id.Id);

    public List<CachedPackage> RetrieveReleaseHistory(PackageURL packageUrl) =>
        (from packages in Db.CachedPackages
         where packages.PackageName == packageUrl.FormatWithoutVersion()
         select packages).ToList();

    public void AddReleaseHistory(List<CachedPackage> cachedPackages)
    {
        foreach (var cachedPackage in cachedPackages)
        {
            if (Db.CachedPackages.FirstOrDefault(package => package.PackageUrl == cachedPackage.PackageUrl) != null)
            {
                continue;
            }

            Db.CachedPackages.Add(cachedPackage);
            Db.SaveChanges();
        }
    }

    public void AddHistoryIntervalStop(CachedHistoryIntervalStop historyIntervalStop)
    {
        Db.CachedHistoryIntervalStops.Add(historyIntervalStop);
        Db.SaveChanges();
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
