namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IGitManager
{
    public GitCommitIdentifier ParseCommitId(string commitId);

    string CreateArchive(string repositoryId, GitCommitIdentifier gitCommitIdentifier);
}
