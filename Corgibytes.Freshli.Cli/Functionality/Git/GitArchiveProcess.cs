using System.Diagnostics;
using System.IO;

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

        // If it exists, make sure to empty it so we are certain we start with a clean slate.
        if (Directory.Exists(historiesDirectoryPath.FullName + "/" + gitCommitIdentifier))
        {
            Directory.Delete(historiesDirectoryPath.FullName + "/" + gitCommitIdentifier);
        }

        // Create the directory where we want to place the archive
        historiesDirectoryPath.CreateSubdirectory(gitCommitIdentifier.ToString());
        var gitSourceTarget = new DirectoryInfo(historiesDirectoryPath.FullName + "/" + gitCommitIdentifier);

        var cloneProcess = new Process
        {
            StartInfo = new()
            {
                FileName = gitPath,
                WorkingDirectory = gitSource.Directory.FullName,
                Arguments = $"git archive --output={gitSourceTarget.FullName}/archive.zip --format=zip {gitCommitIdentifier}",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        cloneProcess.Start();
        cloneProcess.WaitForExit();

        if (cloneProcess.ExitCode != 0)
        {
            throw new GitException("Uh-oh");
        }

        return "loremipsumdonec";
    }
}

