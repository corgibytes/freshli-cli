using System;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitManager : IGitManager
{
    [JsonProperty] private readonly IConfiguration _configuration;
    [JsonProperty] private readonly GitArchive _gitArchive;
    [JsonProperty] private readonly IInvoke _invoke;

    public GitManager(IInvoke invoke, GitArchive gitArchive, IConfiguration configuration)
    {
        _invoke = invoke;
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
            _invoke.Command(_configuration.GitPath, "status", repositoryLocation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public GitCommitIdentifier ParseCommitId(string commitId) => new(commitId);
}
