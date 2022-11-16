namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IGitManager
{
    public GitCommitIdentifier ParseCommitId(string commitId);

    // TODO: Make this method return ValueTask<string>
    string CreateArchive(string repositoryId, GitCommitIdentifier gitCommitIdentifier);

    // TODO: Make this method return ValueTask<bool>
    public bool IsGitRepositoryInitialized(string repositoryLocation);
}
