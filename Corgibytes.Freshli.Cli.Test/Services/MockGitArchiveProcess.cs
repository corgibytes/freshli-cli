using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Test.Services;

public class MockGitArchiveProcess : IGitArchiveProcess
{
    public string Run(GitSource gitSource, GitCommitIdentifier gitCommitIdentifier, string gitPath, DirectoryInfo cacheDirectory)
    {
        return "tmp/.freshli/histories/" + gitSource.Hash + "/" + gitCommitIdentifier;
    }
}

