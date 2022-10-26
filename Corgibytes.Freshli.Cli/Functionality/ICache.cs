using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheDb
{
    public Guid SaveAnalysis(CachedAnalysis analysis);
    public CachedAnalysis? RetrieveAnalysis(Guid id);
    public int AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint);
    public int AddPackageLibYear(CachedPackageLibYear packageLibYear);
    public CachedGitSource? RetrieveCachedGitSource(CachedGitSourceId id);
    public void AddCachedGitSource(CachedGitSource cachedGitSource);
    public void RemoveCachedGitSource(CachedGitSource cachedGitSource);
    public CachedHistoryStopPoint? RetrieveHistoryStopPoint(int historyStopPointId);
    public CachedPackageLibYear? RetrievePackageLibYear(int packageLibYearId);
    public List<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl);
    public void StoreCachedReleaseHistory(List<CachedPackage> packages);
}
