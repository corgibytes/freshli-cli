using System;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitManager : IGitManager
{
    [JsonProperty] private readonly GitArchive _gitArchive;

    public GitManager(GitArchive gitArchive)
    {
        _gitArchive = gitArchive;
    }

    public string CreateArchive(
        string repositoryId, GitCommitIdentifier gitCommitIdentifier) =>
        _gitArchive.CreateArchive(repositoryId, gitCommitIdentifier);

    public bool GitRepositoryInitialized(string repositoryLocation, IConfiguration configuration)
    {
        try
        {
            Invoke.Command(configuration.GitPath, "status", repositoryLocation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public GitCommitIdentifier ParseCommitId(string commitId) => new(commitId);
}
