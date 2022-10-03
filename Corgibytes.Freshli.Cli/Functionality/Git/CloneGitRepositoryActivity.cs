using System;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    public CloneGitRepositoryActivity(Guid cachedAnalysisId) => AnalysisId = cachedAnalysisId;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid AnalysisId { get; set; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();

        // Clone or pull the given repository and branch.
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();
            var cachedAnalysis = cacheDb.RetrieveAnalysis(AnalysisId);

            if (cachedAnalysis == null)
            {
                eventClient.Fire(new AnalysisIdNotFoundEvent());
                return;
            }

            var gitRepository =
                eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>()
                    .CloneOrPull(cachedAnalysis.RepositoryUrl, cachedAnalysis.RepositoryBranch);

            var historyStopData = new HistoryStopData(configuration, gitRepository.Id);

            eventClient.Fire(new GitRepositoryClonedEvent
            {
                AnalysisId = AnalysisId,
                HistoryStopData = historyStopData
            });
        }
        catch (GitException e)
        {
            eventClient.Fire(new CloneGitRepositoryFailedEvent { ErrorMessage = e.Message });
        }
    }
}
