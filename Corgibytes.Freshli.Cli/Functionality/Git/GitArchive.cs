using System.IO;
using Corgibytes.Freshli.Cli.Repositories;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitArchive
{
    private readonly IGitArchiveProcess _archiveProcess;
    private readonly ICachedGitSourceRepository _cachedGitSourceRepository;

    public GitArchive(ICachedGitSourceRepository cachedGitSourceRepository, IGitArchiveProcess archiveProcess)
    {
        _cachedGitSourceRepository = cachedGitSourceRepository;
        _archiveProcess = archiveProcess;
    }

    public string CreateArchive(string repositoryId, DirectoryInfo cacheDirectory,
        GitCommitIdentifier gitCommitIdentifier, string gitPath)
    {
        GitSource gitSource = new(repositoryId, cacheDirectory, _cachedGitSourceRepository);
        return _archiveProcess.Run(gitSource, gitCommitIdentifier, gitPath, cacheDirectory);
    }
}
