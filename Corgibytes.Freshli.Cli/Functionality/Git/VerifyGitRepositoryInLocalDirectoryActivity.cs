using System;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class VerifyGitRepositoryInLocalDirectoryActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var gitSourceRepository = eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>();
        var cacheDb = cacheManager.GetCacheDb();
        var analysis = cacheDb.RetrieveAnalysis(AnalysisId);

        if (analysis == null)
        {
            return;
        }

        if (new DirectoryInfo(analysis.RepositoryUrl).Exists == false)
        {
            eventClient.Fire(new DirectoryDoesNotExistFailureEvent
            {
                ErrorMessage = $"Directory does not exist at {analysis.RepositoryUrl}"
            });
            return;
        }

        if (gitManager.GitRepositoryInitialized(analysis.RepositoryUrl, configuration) == false)
        {
            eventClient.Fire(new DirectoryIsNotGitInitializedFailureEvent
            {
                ErrorMessage = $"Directory is not a git initialised directory at {analysis.RepositoryUrl}"
            });
            return;
        }

        var cachedGitSourceId = new CachedGitSourceId(analysis.RepositoryUrl);

        var entry = cacheDb.RetrieveCachedGitSource(cachedGitSourceId);
        if (entry == null)
        {
            var cachedGitSource = new CachedGitSource(cachedGitSourceId.Id, analysis.RepositoryUrl, null,
                analysis.RepositoryUrl);
            gitSourceRepository.Save(cachedGitSource);
        }

        var analysisLocation =
            new AnalysisLocation(configuration, cachedGitSourceId.Id) { LocalDirectory = analysis.RepositoryUrl };

        eventClient.Fire(new GitRepositoryInLocalDirectoryVerifiedEvent
        {
            AnalysisId = analysis.Id,
            AnalysisLocation = analysisLocation
        });
    }
}
