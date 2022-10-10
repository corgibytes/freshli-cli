using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheDb
{
    public Guid SaveAnalysis(CachedAnalysis analysis);
    public CachedAnalysis? RetrieveAnalysis(Guid id);
    public int AddHistoryIntervalStop(CachedHistoryIntervalStop historyIntervalStop);
    public void AddLibYear(CachedLibYear libYear);
    public CachedGitSource? RetrieveCachedGitSource(CachedGitSourceId id);
}
