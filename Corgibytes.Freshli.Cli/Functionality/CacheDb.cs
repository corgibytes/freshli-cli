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

    public CachedHistoryStopPoint? RetrieveHistoryStopPoint(int historyStopPointId) =>
        Db.CachedHistoryStopPoints.Find(historyStopPointId);

    public int AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint)
    {
        var savedEntity = Db.CachedHistoryStopPoints.Add(historyStopPoint);
        Db.SaveChanges();
        return savedEntity.Entity.Id;
    }

    public CachedPackageLibYear? RetrievePackageLibYear(int packageLibYearId) => Db.CachedPackageLibYears.Find(packageLibYearId);

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
