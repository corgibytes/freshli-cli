using System;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly Guid _analysisId;
    [JsonProperty] private readonly string? _branch;

    [JsonProperty] private readonly string _cacheDir;
    [JsonProperty] private readonly string _gitPath;

    [JsonProperty] private readonly string _repoUrl;

    public CloneGitRepositoryActivity(string repoUrl, string? branch, string cacheDir, string gitPath,
        Guid analysisId = new())
    {
        _repoUrl = repoUrl;
        _branch = branch;
        _cacheDir = cacheDir;
        _gitPath = gitPath;
        _analysisId = analysisId;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Clone or pull the given repository and branch.
        try
        {
            // Null warning suppressed, if we can't fetch it from the service container we got way bigger problems.
            var gitRepository =
                eventClient.ServiceProvider.GetService<ICachedGitSourceRepository>()!.CloneOrPull(_repoUrl, _branch,
                    _cacheDir, _gitPath);
            eventClient.Fire(new GitRepositoryClonedEvent
            {
                GitRepositoryId = gitRepository.Id,
                AnalysisId = _analysisId
            });
        }
        catch (GitException e)
        {
            eventClient.Fire(new CloneGitRepositoryFailedEvent { ErrorMessage = e.Message });
        }
    }
}
