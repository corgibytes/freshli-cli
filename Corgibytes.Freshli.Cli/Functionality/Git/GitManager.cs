using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitManager : IGitManager
{
    private readonly GitArchive _gitArchive;

    public GitManager(GitArchive gitArchive)
    {
        _ = gitArchive ?? throw new ArgumentNullException(nameof(gitArchive));
        _gitArchive = gitArchive;
    }

    public string CreateArchive(
        string repositoryId, string cacheDirectory, GitCommitIdentifier gitCommitIdentifier, string gitPath)
    {
        return _gitArchive.CreateArchive(repositoryId, cacheDirectory, gitCommitIdentifier, gitPath);
    }

    public GitCommitIdentifier ParseCommitSha(string commitSha) => new GitCommitIdentifier(commitSha);
}
