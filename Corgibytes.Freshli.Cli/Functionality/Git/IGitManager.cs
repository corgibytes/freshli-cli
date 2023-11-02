using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IGitManager
{
    GitCommitIdentifier ParseCommitId(string commitId);
    ValueTask<string> CreateArchive(string repositoryId, GitCommitIdentifier gitCommitIdentifier);
    ValueTask<bool> IsGitRepositoryInitialized(string repositoryLocation);
    ValueTask<bool> IsWorkingDirectoryClean(string repositoryLocation);
    ValueTask<string> GetBranchName(string repositoryLocation);
    ValueTask<string> GetRemoteUrl(string repositoryLocation);
}
