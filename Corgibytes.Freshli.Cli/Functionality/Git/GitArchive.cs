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

        return await CreateArchiveTask(gitCommitIdentifier, gitSourceTarget, cachedGitSource).AsTask();
    }

    private async ValueTask<string> CreateArchiveTask(GitCommitIdentifier gitCommitIdentifier,
        DirectoryInfo gitSourceTarget, CachedGitSource gitSource)
    {
        if (await Task.Run(() => Directory.Exists(gitSourceTarget.FullName)))
        {
            return gitSourceTarget.FullName;
        }

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
