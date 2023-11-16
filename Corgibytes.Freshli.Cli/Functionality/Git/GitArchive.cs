using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Support;
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
        // TODO: Modify this so that it does the following:
        // 1. Keep the zip file around in the cache directory - it's a pristine copy of the source code at the commit
        // 2. If the zip doesn't already exist then run `git archive` to create it
        // 3. Remove the expanded directory if it already exists
        // 4. Expand the zip file into the cache directory
        // This should speed things up a little bit

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
