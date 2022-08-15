using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class MockListCommits : IListCommits
{
    private IEnumerable<GitCommit> _availableCommits;

    public MockListCommits() => _availableCommits = new List<GitCommit>();

    public IEnumerable<GitCommit> ForRepository(string repositoryId, string cacheDir, string gitPath) =>
        _availableCommits;

    public void HasCommitsAvailable(IEnumerable<GitCommit> availableGitCommits) =>
        _availableCommits = availableGitCommits;
}
