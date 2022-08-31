namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IGitManager
{
    public GitCommitIdentifier ParseCommitSha(string commitSha);

    string CreateArchive(string repositoryId, string cacheDirectory, GitCommitIdentifier gitCommitIdentifier,
        string gitPath);
}
