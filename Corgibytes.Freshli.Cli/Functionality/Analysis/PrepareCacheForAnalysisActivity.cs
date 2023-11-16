using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class PrepareCacheForAnalysisActivity : ApplicationActivityBase
{
    public PrepareCacheForAnalysisActivity(string repositoryUrl = "", string? repositoryBranch = null,
        string historyInterval = "", CommitHistory useCommitHistory = CommitHistory.AtInterval,
        RevisionHistoryMode revisionHistoryMode = RevisionHistoryMode.AllRevisions)
    {
        RepositoryUrl = repositoryUrl;
        RepositoryBranch = repositoryBranch;
        HistoryInterval = historyInterval;
        UseCommitHistory = useCommitHistory;
        RevisionHistoryMode = revisionHistoryMode;
    }

    public string RepositoryUrl { get; init; }
    public string? RepositoryBranch { get; init; }
    public CommitHistory UseCommitHistory { get; init; }
    public RevisionHistoryMode RevisionHistoryMode { get; init; }

    // TODO: Research how to use a value class here instead of a string
    public string HistoryInterval { get; init; }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();

        try
        {
            if (await cacheManager.Prepare())
            {
                await eventClient.Fire(
                    new CachePreparedForAnalysisEvent
                    {
                        RepositoryUrl = RepositoryUrl,
                        RepositoryBranch = RepositoryBranch,
                        HistoryInterval = HistoryInterval,
                        UseCommitHistory = UseCommitHistory,
                        RevisionHistoryMode = RevisionHistoryMode
                    },
                    cancellationToken);
            }
            else
            {
                await eventClient.Fire(
                    new CachePrepareFailedForAnalysisEvent
                    {
                        ErrorMessage = "Failed to prepare the cache for an unknown reason"
                    },
                    cancellationToken);
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(
                new CachePrepareFailedForAnalysisEvent
                {
                    ErrorMessage = error.Message,
                    Exception = error
                },
                cancellationToken);
        }
    }
}
