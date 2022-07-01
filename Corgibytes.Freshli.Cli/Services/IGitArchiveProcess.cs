using System.IO;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Services;

public interface IGitArchiveProcess
{
    public string Run(GitRepository gitRepository, GitCommitIdentifier gitCommitIdentifier, string gitPath, DirectoryInfo cacheDirectory);
}
