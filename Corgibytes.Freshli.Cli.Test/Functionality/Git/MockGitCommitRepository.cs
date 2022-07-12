using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class MockGitCommitRepository : IGitCommitRepository
{
    private IEnumerable<GitCommit> _availableCommits;

    public MockGitCommitRepository() => _availableCommits = new List<GitCommit>();

    public IEnumerable<GitCommit> ListCommits(string repositoryId, string gitPath) => _availableCommits;

    public void HasCommitsAvailable(IEnumerable<GitCommit> availableGitCommits) =>
        _availableCommits = availableGitCommits;
}
