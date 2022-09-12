using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CachePreparedEvent : IApplicationEvent
{
    public string GitPath { get; init; } = null!;
    public string CacheDirectory { get; init; } = null!;
    public string RepositoryUrl { get; init; } = null!;
    public string? RepositoryBranch { get; init; }
    public string HistoryInterval { get; init; } = null!;
    public CommitHistory UseCommitHistory { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new RestartAnalysisActivity(
            eventClient.ServiceProvider.GetRequiredService<ICacheManager>(),
            eventClient.ServiceProvider.GetRequiredService<IHistoryIntervalParser>()
        )
        {
            GitPath = GitPath,
            CacheDirectory = CacheDirectory,
            RepositoryUrl = RepositoryUrl,
            RepositoryBranch = RepositoryBranch,
            HistoryInterval = HistoryInterval,
            UseCommitHistory = UseCommitHistory
        });
}
