using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitArchive
{
    private static readonly Dictionary<string, Task<string>> s_gitIdsAndSourceTargets = new();
    private static readonly SemaphoreSlim s_gitIdsAndSourceTargetsLock = new(1, 1);
    private readonly ICachedGitSourceRepository _cachedGitSourceRepository;

    private readonly IConfiguration _configuration;

    public GitArchive(IConfiguration configuration, ICachedGitSourceRepository cachedGitSourceRepository)
    {
        _configuration = configuration;
        _cachedGitSourceRepository = cachedGitSourceRepository;
    }

    public async ValueTask<string> CreateArchive(string repositoryId, GitCommitIdentifier gitCommitIdentifier)
    {
        var cachedGitSource = await _cachedGitSourceRepository.FindOneByRepositoryId(repositoryId);

        var gitSourceTarget = new DirectoryInfo(Path.Combine(_configuration.CacheDir, "histories", cachedGitSource.Id,
            gitCommitIdentifier.ToString()));

        Task<string>? createArchiveTask = null;

        await s_gitIdsAndSourceTargetsLock.WaitAsync();
        try
        {
            if (s_gitIdsAndSourceTargets.ContainsKey(gitCommitIdentifier.ToString()))
            {
                createArchiveTask = s_gitIdsAndSourceTargets[gitCommitIdentifier.ToString()];
            }
        }
        finally
        {
            s_gitIdsAndSourceTargetsLock.Release();
        }

        if (createArchiveTask != null)
        {
            return await createArchiveTask;
        }

        await s_gitIdsAndSourceTargetsLock.WaitAsync();
        try
        {
            if (await Task.Run(() => Directory.Exists(gitSourceTarget.FullName)))
            {
                return gitSourceTarget.FullName;
            }

            s_gitIdsAndSourceTargets.Add(
                gitCommitIdentifier.ToString(),
                CreateArchiveTask(gitCommitIdentifier, gitSourceTarget, cachedGitSource).AsTask()
            );

            createArchiveTask = s_gitIdsAndSourceTargets[gitCommitIdentifier.ToString()];
        }
        finally
        {
            s_gitIdsAndSourceTargetsLock.Release();
        }

        return await createArchiveTask;
    }

    private async ValueTask<string> CreateArchiveTask(GitCommitIdentifier gitCommitIdentifier,
        DirectoryInfo gitSourceTarget, CachedGitSource gitSource)
    {
        // Create the directory where we want to place the archive
        await Task.Run(gitSourceTarget.Create);
        var archivePath = Path.Combine(gitSourceTarget.FullName, "archive.zip");

        // TODO: This should use the CommandInvoker class
        var archiveProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _configuration.GitPath,
                WorkingDirectory = gitSource.LocalPath,
                Arguments = $"archive --output={archivePath} --format=zip {gitCommitIdentifier}",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        archiveProcess.Start();
        await archiveProcess.WaitForExitAsync();

        if (archiveProcess.ExitCode != 0)
        {
            throw new GitException(
                string.Format(CliOutput.GitArchive_Git_Exception,
                await archiveProcess.StandardError.ReadToEndAsync()));
        }

        await Task.Run(() => ZipFile.ExtractToDirectory($"{archivePath}", gitSourceTarget.FullName));
        await Task.Run(() => File.Delete($"{archivePath}"));

        return gitSourceTarget.FullName;
    }
}
