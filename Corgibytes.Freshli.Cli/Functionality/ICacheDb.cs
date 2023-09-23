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
    public ValueTask<CachedHistoryStopPoint> AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint);
    public ValueTask<CachedManifest> AddManifest(CachedHistoryStopPoint historyStopPoint, string manifestFilePath);
    // ReSharper disable once UnusedMemberInSuper.Global
    public ValueTask<CachedManifest?> RetrieveManifest(CachedHistoryStopPoint historyStopPoint, string manifestFilePath);
    public ValueTask<CachedPackageLibYear> AddPackageLibYear(CachedManifest manifest, CachedPackageLibYear packageLibYear);
    public ValueTask<CachedGitSource?> RetrieveCachedGitSource(CachedGitSourceId id);
    public ValueTask AddCachedGitSource(CachedGitSource cachedGitSource);
    public ValueTask RemoveCachedGitSource(CachedGitSource cachedGitSource);
    public ValueTask<CachedHistoryStopPoint?> RetrieveHistoryStopPoint(int historyStopPointId);
    public ValueTask<CachedPackageLibYear?> RetrievePackageLibYear(PackageURL packageUrl, DateTimeOffset asOfDateTime);
    public IAsyncEnumerable<CachedPackage> RetrieveCachedReleaseHistory(PackageURL packageUrl);
    // ReSharper disable once ParameterTypeCanBeEnumerable.Global
    public ValueTask StoreCachedReleaseHistory(List<CachedPackage> packages);
}
