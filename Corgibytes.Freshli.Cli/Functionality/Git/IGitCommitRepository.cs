using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IGitCommitRepository
{
    public IEnumerable<GitCommit> ListCommits(string repositoryId, string gitPath);
}
