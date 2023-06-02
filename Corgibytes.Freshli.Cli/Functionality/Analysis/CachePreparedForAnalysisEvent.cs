using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class CachePreparedForAnalysisEvent : ApplicationEventBase
{
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }
    public RevisionHistoryMode RevisionHistoryMode { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken) =>
        await eventClient.Dispatch(new RestartAnalysisActivity
        {
            RepositoryUrl = RepositoryUrl,
            RepositoryBranch = RepositoryBranch,
            HistoryInterval = HistoryInterval,
            UseCommitHistory = UseCommitHistory,
            RevisionHistoryMode = RevisionHistoryMode
        },
        cancellationToken);
}
