using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class CacheDoesNotExistEvent : ErrorEvent
{
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }
    public RevisionHistoryMode RevisionHistoryMode { get; init; }

    public override void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(
            new PrepareCacheForAnalysisActivity(RepositoryUrl, RepositoryBranch, HistoryInterval, UseCommitHistory)
            {
                RepositoryUrl = RepositoryUrl,
                RepositoryBranch = RepositoryBranch,
                HistoryInterval = HistoryInterval,
                UseCommitHistory = UseCommitHistory,
                RevisionHistoryMode = RevisionHistoryMode
            });
}
