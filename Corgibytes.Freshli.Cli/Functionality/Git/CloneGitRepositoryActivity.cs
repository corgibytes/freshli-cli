using System;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly Guid _analysisId;

    [JsonProperty] private readonly string _cacheDir;
    [JsonProperty] private readonly string _gitPath;

    public CloneGitRepositoryActivity(string repoUrl, string? branch, string cacheDir, string gitPath,
        Guid analysisId = new())
    {
        RepoUrl = repoUrl;
        Branch = branch;
        _cacheDir = cacheDir;
        _gitPath = gitPath;
        _analysisId = analysisId;
    }

    [JsonProperty] private string? Branch { get; set; }
    [JsonProperty] private string RepoUrl { get; set; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Clone or pull the given repository and branch.
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb(_cacheDir);
            var cachedAnalysis = cacheDb.RetrieveAnalysis(_analysisId);
            if (cachedAnalysis != null)
            {
                RepoUrl = cachedAnalysis.RepositoryUrl;
                Branch = cachedAnalysis.RepositoryBranch;
            }

            var gitRepository =
                eventClient.ServiceProvider.GetRequiredService<ICachedGitSourceRepository>()
                    .CloneOrPull(RepoUrl, Branch, _cacheDir, _gitPath);

            var analysisLocation = new AnalysisLocation(_cacheDir, gitRepository.Id);

            eventClient.Fire(new GitRepositoryClonedEvent
            {
                AnalysisId = _analysisId,
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
