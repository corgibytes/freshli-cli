using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class VerifyGitRepositoryInLocalDirectoryActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var gitSourceRepository = eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>();
        var cacheDb = await cacheManager.GetCacheDb();
        var analysis = await cacheDb.RetrieveAnalysis(AnalysisId);

        if (analysis == null)
        {
            return;
        }

        if (new DirectoryInfo(analysis.RepositoryUrl).Exists == false)
        {
            await eventClient.Fire(
                new DirectoryDoesNotExistFailureEvent
                {
                    ErrorMessage = $"Directory does not exist at {analysis.RepositoryUrl}"
                },
                cancellationToken);
            return;
        }

        if (await gitManager.IsGitRepositoryInitialized(analysis.RepositoryUrl) == false)
        {
            await eventClient.Fire(
                new DirectoryIsNotGitInitializedFailureEvent
                {
                    ErrorMessage = $"Directory is not a git initialised directory at {analysis.RepositoryUrl}"
                },
                cancellationToken);
            return;
        }

        var cachedGitSourceId = new CachedGitSourceId(analysis.RepositoryUrl);

        var entry = await cacheDb.RetrieveCachedGitSource(cachedGitSourceId);
        if (entry == null)
        {
            var cachedGitSource = new CachedGitSource
            {
                Id = cachedGitSourceId.Id,
                // TODO: This needs to retrieve the default git remote if `analysis.RepositoryUrl` is a local directory.
                Url = analysis.RepositoryUrl,
                // TODO: This needs to retrieve the current branch name (or commit sha if detached head).
                Branch = null,
                // TODO: Need to ensure that this is a full expanded path.
                LocalPath = analysis.RepositoryUrl
            };
            await gitSourceRepository.Save(cachedGitSource);
        }

        var historyStopData =
            new HistoryStopData
            {
                Configuration = configuration,
                RepositoryId = cachedGitSourceId.Id,
                LocalDirectory = analysis.RepositoryUrl
            };

        await eventClient.Fire(
            new GitRepositoryInLocalDirectoryVerifiedEvent
            {
                AnalysisId = analysis.Id,
                HistoryStopData = historyStopData
            },
            cancellationToken);
    }
}
