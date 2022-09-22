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
    public string CacheDir { get; }
    public string GitPath { get; }

    public CloneGitRepositoryActivity(Guid cachedAnalysisId,  string cacheDir, string gitPath)
    {
        CacheDir = cacheDir;
        GitPath = gitPath;
        AnalysisId = cachedAnalysisId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Clone or pull the given repository and branch.
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb(CacheDir);
            var cachedAnalysis = cacheDb.RetrieveAnalysis(AnalysisId);

            var gitRepository =
                eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>()
                    .CloneOrPull(cachedAnalysis!.RepositoryUrl, cachedAnalysis.RepositoryBranch, CacheDir, GitPath);

            var analysisLocation = new AnalysisLocation(CacheDir, gitRepository.Id);

            eventClient.Fire(new GitRepositoryClonedEvent
            {
                AnalysisId = AnalysisId,
                GitPath = GitPath,
                AnalysisLocation = analysisLocation
            });
        }
        catch (GitException e)
        {
            eventClient.Fire(new CloneGitRepositoryFailedEvent { ErrorMessage = e.Message });
        }
    }
}
