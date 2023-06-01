using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    public readonly Guid CachedAnalysisId;

    public CloneGitRepositoryActivity(Guid cachedAnalysisId) => CachedAnalysisId = cachedAnalysisId;

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();

        // Clone or pull the given repository and branch.
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();
            var cachedAnalysis = await cacheDb.RetrieveAnalysis(CachedAnalysisId);

            if (cachedAnalysis == null)
            {
                await eventClient.Fire(new AnalysisIdNotFoundEvent(), cancellationToken);
                return;
            }

            await eventClient.Fire(
                new GitRepositoryCloneStartedEvent { AnalysisId = CachedAnalysisId },
                cancellationToken);

            var gitRepositoryService = eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>();
            var gitRepository = await gitRepositoryService.CloneOrPull(
                cachedAnalysis.RepositoryUrl, cachedAnalysis.RepositoryBranch);

            var historyStopData = new HistoryStopData(configuration, gitRepository.Id);

            await eventClient.Fire(
                new GitRepositoryClonedEvent
                {
                    AnalysisId = CachedAnalysisId,
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
