using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitManager : IGitManager
{
    [JsonProperty] private readonly GitArchive _gitArchive;

    public GitManager(GitArchive gitArchive) => _gitArchive = gitArchive;

    public string CreateArchive(
        string repositoryId, string cacheDirectory, GitCommitIdentifier gitCommitIdentifier, string gitPath) =>
        _gitArchive.CreateArchive(repositoryId, cacheDirectory, gitCommitIdentifier, gitPath);

    public GitCommitIdentifier ParseCommitId(string commitId) => new(commitId);
}
