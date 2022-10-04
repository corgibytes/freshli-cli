using System;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitManager : IGitManager
{
    [JsonProperty] private readonly GitArchive _gitArchive;
    [JsonProperty] private readonly IConfiguration _configuration;

    public GitManager(GitArchive gitArchive, IConfiguration configuration)
    {
        _gitArchive = gitArchive;
        _configuration = configuration;
    }

    public string CreateArchive(
        string repositoryId, GitCommitIdentifier gitCommitIdentifier) =>
        _gitArchive.CreateArchive(repositoryId, gitCommitIdentifier);

    public bool IsGitRepositoryInitialized(string repositoryLocation)
    {
        try
        {
            Invoke.Command(_configuration.GitPath, "status", repositoryLocation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public GitCommitIdentifier ParseCommitId(string commitId) => new(commitId);
}
