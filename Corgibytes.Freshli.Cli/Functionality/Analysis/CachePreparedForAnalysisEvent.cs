using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class CachePreparedForAnalysisEvent : IApplicationEvent
{
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }
    public RevisionHistoryMode RevisionHistoryMode { get; init; }

    public async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(new RestartAnalysisActivity
        {
            RepositoryUrl = RepositoryUrl,
            RepositoryBranch = RepositoryBranch,
            HistoryInterval = HistoryInterval,
            UseCommitHistory = UseCommitHistory,
            RevisionHistoryMode = RevisionHistoryMode
        });
}
