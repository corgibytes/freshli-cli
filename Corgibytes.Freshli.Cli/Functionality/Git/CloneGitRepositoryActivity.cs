using System;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly Guid _cachedAnalysisId;

    [JsonProperty] private readonly string _cacheDir;
    [JsonProperty] private readonly string _gitPath;

    public CloneGitRepositoryActivity(Guid cachedAnalysisId,  string cacheDir, string gitPath)
    {
        _cacheDir = cacheDir;
        _gitPath = gitPath;
        _cachedAnalysisId = cachedAnalysisId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Clone or pull the given repository and branch.
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb(_cacheDir);
            var cachedAnalysis = cacheDb.RetrieveAnalysis(_cachedAnalysisId);

            var gitRepository =
                eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>()
                    .CloneOrPull(cachedAnalysis!.RepositoryUrl, cachedAnalysis.RepositoryBranch, _cacheDir, _gitPath);

            var analysisLocation = new AnalysisLocation(_cacheDir, gitRepository.Id);

            eventClient.Fire(new GitRepositoryClonedEvent
            {
                AnalysisId = _cachedAnalysisId,
                GitPath = _gitPath,
                AnalysisLocation = analysisLocation
            });
        }
        catch (GitException e)
        {
            eventClient.Fire(new CloneGitRepositoryFailedEvent { ErrorMessage = e.Message });
        }
    }
}
