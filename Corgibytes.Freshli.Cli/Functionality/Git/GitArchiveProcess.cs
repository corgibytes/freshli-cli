using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitArchiveProcess : IGitArchiveProcess
{
    public string Run(GitSource gitSource, GitCommitIdentifier gitCommitIdentifier, string gitPath, DirectoryInfo cacheDirectory)
    {
        var historiesDirectoryPath = new DirectoryInfo(cacheDirectory.FullName + "/histories");
        if (!Directory.Exists(historiesDirectoryPath.FullName))
        {
            cacheDirectory.CreateSubdirectory("histories");
        }

        var repositoryHistoriesDirectoryPath = new DirectoryInfo(cacheDirectory.FullName + "/histories/" + gitSource.Hash);
        if (!Directory.Exists(repositoryHistoriesDirectoryPath.FullName))
        {
            historiesDirectoryPath.CreateSubdirectory(gitSource.Hash);
        }

        // If it exists, make sure to empty it so we are certain we start with a clean slate.
        var gitSourceTarget = new DirectoryInfo(historiesDirectoryPath.FullName + "/" + gitSource.Hash + "/" + gitCommitIdentifier);
        if (Directory.Exists(gitSourceTarget.FullName))
        {
            Directory.Delete(gitSourceTarget.FullName, recursive: true);
        }

        // Create the directory where we want to place the archive
        repositoryHistoriesDirectoryPath.CreateSubdirectory(gitCommitIdentifier.ToString());
        var archivePath = $"{gitSourceTarget.FullName}/archive.zip";

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

