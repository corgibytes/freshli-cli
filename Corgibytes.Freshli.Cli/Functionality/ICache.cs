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
    void AddHistoryIntervalStop(CachedHistoryIntervalStop cachedHistoryIntervalStop);
    public CachedGitSource? RetrieveCachedGitSource(CachedGitSourceId id);
    public List<CachedPackage> RetrieveReleaseHistory(PackageURL packageUrl);
    public void AddReleaseHistory(List<CachedPackage> cachedPackages);
}
