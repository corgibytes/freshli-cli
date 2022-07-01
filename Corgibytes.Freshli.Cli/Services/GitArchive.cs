using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Repositories;

namespace Corgibytes.Freshli.Cli.Services;

public class GitArchive
{
    private readonly ICachedGitSourceRepository _cachedGitSourceRepository;
    private readonly IGitArchiveProcess _archiveProcess;

    public GitArchive(ICachedGitSourceRepository cachedGitSourceRepository, IGitArchiveProcess archiveProcess)
    {
        _cachedGitSourceRepository = cachedGitSourceRepository;
        _archiveProcess = archiveProcess;
    }

    public string CreateArchive(string repositoryId, DirectoryInfo cacheDirectory, GitCommitIdentifier gitCommitIdentifier, string gitPath)
    {
        GitRepository gitRepository = new(repositoryId, cacheDirectory, _cachedGitSourceRepository);
        return _archiveProcess.Run(gitRepository, gitCommitIdentifier, gitPath, cacheDirectory);
    }
}

