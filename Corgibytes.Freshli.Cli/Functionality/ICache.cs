using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheDb
{
    public Guid SaveAnalysis(CachedAnalysis analysis);
    public CachedAnalysis? RetrieveAnalysis(Guid id);
    void AddHistoryIntervalStop(CachedHistoryIntervalStop cachedHistoryIntervalStop);
    public CachedGitSource? RetrieveCachedGitSource(CachedGitSourceId id);
}
