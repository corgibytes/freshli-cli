using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Corgibytes.Freshli.Cli.Repositories;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitArchive
{
    private readonly ICachedGitSourceRepository _cachedGitSourceRepository;

    public GitArchive(ICachedGitSourceRepository cachedGitSourceRepository) =>
        _cachedGitSourceRepository = cachedGitSourceRepository;

    public string CreateArchive(string repositoryId, DirectoryInfo cacheDirectory,
        GitCommitIdentifier gitCommitIdentifier, string gitPath)
    {
        var gitSource = new GitSource(repositoryId, cacheDirectory, _cachedGitSourceRepository);

        // If it exists, make sure to empty it so we are certain we start with a clean slate.
        var gitSourceTarget = new DirectoryInfo(Path.Combine(cacheDirectory.FullName, "histories", gitSource.Hash,
            gitCommitIdentifier.ToString()));
        if (Directory.Exists(gitSourceTarget.FullName))
        {
            Directory.Delete(gitSourceTarget.FullName, true);
        }

        // Create the directory where we want to place the archive
        gitSourceTarget.Create();
        var archivePath = Path.Combine(gitSourceTarget.FullName, "archive.zip");

        var archiveProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = gitPath,
                WorkingDirectory = gitSource.Directory.FullName,
                Arguments = $"archive --output={archivePath} --format=zip {gitCommitIdentifier}",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        archiveProcess.Start();
        archiveProcess.WaitForExit();

        if (archiveProcess.ExitCode != 0)
        {
            throw new GitException(string.Format(CliOutput.GitArchive_Git_Exception,
                archiveProcess.StandardError.ReadToEnd()));
        }

        ZipFile.ExtractToDirectory($"{archivePath}", gitSourceTarget.FullName);
        File.Delete($"{archivePath}");

        return gitSourceTarget.FullName;
    }
}
