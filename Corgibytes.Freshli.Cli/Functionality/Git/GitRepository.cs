using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepository : IGitCommitRepository
{
    public IEnumerable<GitCommit> ListCommits(string repositoryId, string gitPath) => throw new System.NotImplementedException();
}
