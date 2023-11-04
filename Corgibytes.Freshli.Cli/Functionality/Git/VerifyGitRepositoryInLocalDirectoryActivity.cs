using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class VerifyGitRepositoryInLocalDirectoryActivity : IApplicationActivity
{
    public required Guid AnalysisId { get; init; }
    public required string ProjectSlug { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        var gitManager = eventClient.ServiceProvider.GetRequiredService<IGitManager>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var gitSourceRepository = eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>();
        var cacheDb = await cacheManager.GetCacheDb();
        var analysis = await cacheDb.RetrieveAnalysis(AnalysisId);

        var localGitDirectory = analysis.RepositoryUrl;

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

        if (await gitManager.IsWorkingDirectoryClean(analysis.RepositoryUrl) == false)
        {
            await eventClient.Fire(
                new DirectoryIsNotGitInitializedFailureEvent
                {
                    ErrorMessage = $"There are pending changes in the git directory at {analysis.RepositoryUrl}"
                },
                cancellationToken);
            return;
        }

        // TODO: Need to ensure that this is a full expanded path

        var gitBranch = await gitManager.GetBranchName(localGitDirectory);
        var gitRemoteUrl = await gitManager.GetRemoteUrl(localGitDirectory);

        var cachedGitSourceId = new CachedGitSourceId(gitRemoteUrl, gitBranch);

        var entry = await cacheDb.RetrieveCachedGitSource(cachedGitSourceId);
        if (entry == null)
        {
            var cachedGitSource = new CachedGitSource
            {
                Id = cachedGitSourceId.Id,
                Url = gitRemoteUrl,
                Branch = gitBranch,
                LocalPath = localGitDirectory
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
