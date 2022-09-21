using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitArchive
{
    private static readonly Dictionary<string, Task<string>> s_gitIdsAndSourceTargets = new();
    private static readonly object s_gitIdsAndSourceTargetsLock = new();

    private readonly ICachedGitSourceRepository _cachedGitSourceRepository;

    public GitArchive(ICachedGitSourceRepository cachedGitSourceRepository) =>
        _cachedGitSourceRepository = cachedGitSourceRepository;

    public string CreateArchive(string repositoryId, string cacheDirectory,
        GitCommitIdentifier gitCommitIdentifier, string gitPath)
    {
        var gitSource = _cachedGitSourceRepository.FindOneByHash(repositoryId, cacheDirectory);

        var gitSourceTarget = new DirectoryInfo(Path.Combine(cacheDirectory, "histories", gitSource.Id,
            gitCommitIdentifier.ToString()));

        Task<string>? createArchiveTask = null;
        lock (s_gitIdsAndSourceTargetsLock)
        {
            if (s_gitIdsAndSourceTargets.ContainsKey(gitSource.Id))
            {
                createArchiveTask = s_gitIdsAndSourceTargets[gitSource.Id];
            }
        }

        if (createArchiveTask != null)
        {
            return createArchiveTask.Result;
        }

        lock (s_gitIdsAndSourceTargetsLock)
        {
            if (Directory.Exists(gitSourceTarget.FullName))
            {
                return gitSourceTarget.FullName;
            }

            s_gitIdsAndSourceTargets.Add(gitSource.Id, new Task<string>(() =>
                CreateArchiveTask(gitCommitIdentifier, gitPath, gitSourceTarget, gitSource)));

            createArchiveTask = s_gitIdsAndSourceTargets[gitSource.Id];
            createArchiveTask.Start();
        }

        return createArchiveTask.Result;
    }

    private static string CreateArchiveTask(GitCommitIdentifier gitCommitIdentifier, string gitPath,
        DirectoryInfo gitSourceTarget, CachedGitSource gitSource)
    {
        // Create the directory where we want to place the archive
        gitSourceTarget.Create();
        var archivePath = Path.Combine(gitSourceTarget.FullName, "archive.zip");

        var archiveProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = gitPath,
                WorkingDirectory = gitSource.LocalPath,
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
