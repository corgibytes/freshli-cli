using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Test.Services;

public class MockGitArchiveProcess : IGitArchiveProcess
{
    public string Run(GitRepository gitRepository, GitCommitIdentifier gitCommitIdentifier, string gitPath, DirectoryInfo cacheDirectory)
    {
        return "tmp/.freshli/histories/" + gitRepository.Hash + "/" + gitCommitIdentifier;
    }
}

