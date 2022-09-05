using System;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly ICachedGitSourceRepository _gitSourceRepository;

    [JsonProperty] private readonly string _cacheDir;
    [JsonProperty] private readonly string _gitPath;
    [JsonProperty] private readonly Guid _analysisId;

    [JsonProperty] private readonly string _repoUrl;
    [JsonProperty] private readonly string? _branch;

    [JsonConstructor]
    public CloneGitRepositoryActivity(ICachedGitSourceRepository gitSourceRepository,
        string repoUrl, string? branch, string cacheDir, string gitPath, Guid analysisId = new())
    {
        _gitSourceRepository = gitSourceRepository;
        _repoUrl = repoUrl;
        _branch = branch;
        _cacheDir = cacheDir;
        _gitPath = gitPath;
        _analysisId = analysisId;
    }

    public CloneGitRepositoryActivity(IServiceProvider serviceProvider, Guid analysisId, string cacheDir, string gitPath)
    {
        _cacheDir = cacheDir;
        _gitPath = gitPath;
        _analysisId = analysisId;

        _gitSourceRepository = serviceProvider.GetRequiredService<ICachedGitSourceRepository>();

        var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb(_cacheDir);
        var cachedAnalysis = cacheDb.RetrieveAnalysis(_analysisId);

        _repoUrl = cachedAnalysis!.RepositoryUrl;
        _branch = cachedAnalysis.RepositoryBranch;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Clone or pull the given repository and branch.
        try
        {
            var gitRepository = _gitSourceRepository.CloneOrPull(_repoUrl, _branch, _cacheDir, _gitPath);
            eventClient.Fire(new GitRepositoryClonedEvent { GitRepositoryId = gitRepository.Id, AnalysisId = _analysisId });
        }
        catch (GitException e)
        {
            eventClient.Fire(new CloneGitRepositoryFailedEvent { ErrorMessage = e.Message });
        }
    }
}
