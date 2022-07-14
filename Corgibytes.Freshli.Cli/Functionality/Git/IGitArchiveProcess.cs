using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IGitArchiveProcess
{
    public string Run(GitSource gitSource, GitCommitIdentifier gitCommitIdentifier, string gitPath,
        DirectoryInfo cacheDirectory);
}
