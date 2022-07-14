using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitArchiveProcess : IGitArchiveProcess
{
    public string Run(GitSource gitSource, GitCommitIdentifier gitCommitIdentifier, string gitPath,
        DirectoryInfo cacheDirectory)
    {
        // If it exists, make sure to empty it so we are certain we start with a clean slate.
        var gitSourceTarget = new DirectoryInfo(Path.Combine(cacheDirectory.FullName, "histories", gitSource.Hash, gitCommitIdentifier.ToString()));
        if (Directory.Exists(gitSourceTarget.FullName))
        {
            Directory.Delete(gitSourceTarget.FullName, true);
        }

        // Create the directory where we want to place the archive
        gitSourceTarget.Create();
        var archivePath = Path.Combine(gitSourceTarget.FullName, "archive.zip");

        var archiveProcess = new Process
        {
            StartInfo = new()
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
            throw new GitException($"Git encountered an error:\n{archiveProcess.StandardError.ReadToEnd()}");
        }

        ZipFile.ExtractToDirectory($"{archivePath}", gitSourceTarget.FullName);
        File.Delete($"{archivePath}");

        return gitSourceTarget.FullName;
    }
}
