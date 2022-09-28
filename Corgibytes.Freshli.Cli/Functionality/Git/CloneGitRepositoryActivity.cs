using System;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    public Guid AnalysisId { get; }

    public CloneGitRepositoryActivity(Guid cachedAnalysisId)
    {
        AnalysisId = cachedAnalysisId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();

        // Clone or pull the given repository and branch.
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();
            var cachedAnalysis = cacheDb.RetrieveAnalysis(AnalysisId);

            var gitRepository =
                eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>()
                    .CloneOrPull(cachedAnalysis!.RepositoryUrl, cachedAnalysis.RepositoryBranch);

            var analysisLocation = new AnalysisLocation(configuration, gitRepository.Id);

            eventClient.Fire(new GitRepositoryClonedEvent
            {
                AnalysisId = AnalysisId,
                AnalysisLocation = analysisLocation
            });
        }
        catch (GitException e)
        {
            eventClient.Fire(new CloneGitRepositoryFailedEvent { ErrorMessage = e.Message });
        }
    }
}
