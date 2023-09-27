using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Support;

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

    public async ValueTask<string> CreateArchive(
        string repositoryId, GitCommitIdentifier gitCommitIdentifier) =>
        await _gitArchive.CreateArchive(repositoryId, gitCommitIdentifier);

    public async ValueTask<bool> IsGitRepositoryInitialized(string repositoryLocation)
    {
        try
        {
            await _commandInvoker.Run(_configuration.GitPath, "status", repositoryLocation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public GitCommitIdentifier ParseCommitId(string commitId) => new(commitId);
}
