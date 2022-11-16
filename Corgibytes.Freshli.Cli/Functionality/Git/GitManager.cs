using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitManager : IGitManager
{
    private readonly ICommandInvoker _commandInvoker;
    private readonly IConfiguration _configuration;
    private readonly GitArchive _gitArchive;

    public GitManager(ICommandInvoker commandInvoker, GitArchive gitArchive, IConfiguration configuration)
    {
        _commandInvoker = commandInvoker;
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
            _commandInvoker.Run(_configuration.GitPath, "status", repositoryLocation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public GitCommitIdentifier ParseCommitId(string commitId) => new(commitId);
}
