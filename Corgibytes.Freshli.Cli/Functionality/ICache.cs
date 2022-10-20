using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheDb
{
    public Guid SaveAnalysis(CachedAnalysis analysis);
    public CachedAnalysis? RetrieveAnalysis(Guid id);
    public int AddHistoryStopPoint(CachedHistoryStopPoint historyStopPoint);
    public int AddPackageLibYear(CachedPackageLibYear packageLibYear);
    public CachedGitSource? RetrieveCachedGitSource(CachedGitSourceId id);
    public CachedHistoryStopPoint? RetrieveHistoryStopPoint(int historyStopPointId);
    public CachedPackageLibYear? RetrievePackageLibYear(int packageLibYearId);
}
