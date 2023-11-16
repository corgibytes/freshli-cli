using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : ApplicationActivityBase
{
    public required Guid AnalysisId { get; init; }
    public required string ProjectSlug { get; init; }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();

        // Clone or pull the given repository and branch.
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = await cacheManager.GetCacheDb();
            var cachedAnalysis = await cacheDb.RetrieveAnalysis(AnalysisId);

            if (cachedAnalysis == null)
            {
                await eventClient.Fire(new AnalysisIdNotFoundEvent(), cancellationToken);
                return;
            }

            await eventClient.Fire(
                new GitRepositoryCloneStartedEvent { AnalysisId = AnalysisId },
                cancellationToken);

            var gitRepositoryService = eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>();
            var gitRepository = await gitRepositoryService.CloneOrPull(
                cachedAnalysis.RepositoryUrl, cachedAnalysis.RepositoryBranch);

            var historyStopData = new HistoryStopData
            {
                Configuration = configuration,
                RepositoryId = gitRepository.Id
            };

            await eventClient.Fire(
                new GitRepositoryClonedEvent
                {
                    AnalysisId = AnalysisId,
                    HistoryStopData = historyStopData
                },
                cancellationToken);
        }
        catch (GitException error)
        {
            await eventClient.Fire(
                new CloneGitRepositoryFailedEvent
                {
                    ErrorMessage = error.Message,
                    Exception = error
                },
                cancellationToken);
        }
    }
}
