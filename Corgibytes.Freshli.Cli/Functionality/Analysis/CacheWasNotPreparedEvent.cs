using Corgibytes.Freshli.Cli.CommandRunners.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class CacheWasNotPreparedEvent : ErrorEvent
{
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }

    public override void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(
            new PrepareCacheActivity(RepositoryUrl, RepositoryBranch, HistoryInterval, UseCommitHistory)
            {
                RepositoryUrl = RepositoryUrl,
                RepositoryBranch = RepositoryBranch,
                HistoryInterval = HistoryInterval,
                UseCommitHistory = UseCommitHistory
            });
}
