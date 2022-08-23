using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryActivity : IApplicationActivity
{
    [JsonProperty] private readonly string? _branch;
    [JsonProperty] private readonly string _cacheDir;
    [JsonProperty] private readonly string _gitPath;
    [JsonProperty] private readonly ICachedGitSourceRepository _gitSourceRepository;
    [JsonProperty] private readonly string _repoUrl;

    public CloneGitRepositoryActivity(ICachedGitSourceRepository gitSourceRepository,
        string repoUrl, string? branch, string cacheDir, string gitPath)
    {
        _gitSourceRepository = gitSourceRepository;
        _repoUrl = repoUrl;
        _branch = branch;
        _cacheDir = cacheDir;
        _gitPath = gitPath;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Clone or pull the given repository and branch.
        try
        {
            var gitRepository = _gitSourceRepository.CloneOrPull(_repoUrl, _branch, _cacheDir, _gitPath);
            eventClient.Fire(new GitRepositoryClonedEvent { GitRepositoryId = gitRepository.Id });
        }
        catch (GitException e)
        {
            eventClient.Fire(new CloneGitRepositoryFailedEvent { ErrorMessage = e.Message });
        }
    }
}
