using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheDb
{
    public ValueTask<Guid> SaveAnalysis(CachedAnalysis analysis);
    public ValueTask<CachedAnalysis?> RetrieveAnalysis(Guid id);
    public ValueTask<int> AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint);
    public ValueTask<int> AddPackageLibYear(CachedPackageLibYear packageLibYear);
    public ValueTask<CachedGitSource?> RetrieveCachedGitSource(CachedGitSourceId id);
    public ValueTask AddCachedGitSource(CachedGitSource cachedGitSource);
    public ValueTask RemoveCachedGitSource(CachedGitSource cachedGitSource);
    public ValueTask<CachedHistoryStopPoint?> RetrieveHistoryStopPoint(int historyStopPointId);
    public ValueTask<CachedPackageLibYear?> RetrievePackageLibYear(int packageLibYearId);
    public IAsyncEnumerable<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl);
    public ValueTask StoreCachedReleaseHistory(List<CachedPackage> packages);
}
