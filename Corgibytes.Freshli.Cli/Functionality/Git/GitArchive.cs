using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
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

    // ReSharper disable once UnusedMember.Global
    public string CreateArchive(string repositoryId, string cacheDirectory,
        GitCommitIdentifier gitCommitIdentifier, string gitPath)
    {
        var gitSource = _cachedGitSourceRepository.FindOneByHash(repositoryId, cacheDirectory);

        // If it exists, make sure to empty it so we are certain we start with a clean slate.
        var gitSourceTarget = new DirectoryInfo(Path.Combine(cacheDirectory, "histories", gitSource.Id,
            gitCommitIdentifier.ToString()));

        Task<string>? createTask = null;
        lock (s_gitIdsAndSourceTargetsLock)
        {
            if (s_gitIdsAndSourceTargets.ContainsKey(gitSource.Id))
            {
                createTask = s_gitIdsAndSourceTargets[gitSource.Id];
            }
        }

        if (createTask != null)
        {
            return createTask.Result;
        }

        lock (s_gitIdsAndSourceTargetsLock)
        {
            s_gitIdsAndSourceTargets.Add(gitSource.Id, new Task<string>(() =>
            {
                if (Directory.Exists(gitSourceTarget.FullName))
                {
                    return gitSourceTarget.FullName;
                }

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
            }));
        }

        lock (s_gitIdsAndSourceTargetsLock)
        {
            createTask = s_gitIdsAndSourceTargets[gitSource.Id];
            createTask.Start();
        }

        return createTask.Result;
    }
}
