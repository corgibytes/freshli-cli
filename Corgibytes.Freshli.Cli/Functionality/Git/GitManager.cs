using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitManager : IGitManager
{
    [JsonProperty] private readonly IConfiguration _configuration;
    [JsonProperty] private readonly GitArchive _gitArchive;

    public GitManager(IConfiguration configuration, GitArchive gitArchive)
    {
        _configuration = configuration;
        _gitArchive = gitArchive;
    }

    public string CreateArchive(
        string repositoryId, GitCommitIdentifier gitCommitIdentifier) =>
        _gitArchive.CreateArchive(repositoryId, gitCommitIdentifier);

    public GitCommitIdentifier ParseCommitId(string commitId) => new(commitId);
}
