using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class MockListCommits : IListCommits
{
    private IEnumerable<GitCommit> _availableCommits = new List<GitCommit>();

    public IEnumerable<GitCommit> ForRepository(IHistoryStopData historyStopData) =>
        _availableCommits;

    public GitCommit MostRecentCommit(IHistoryStopData historyStopData) =>
        // Assuming _availableCommits is sorted with most recent first
        _availableCommits.First();

    public void HasCommitsAvailable(IEnumerable<GitCommit> availableGitCommits) =>
        _availableCommits = availableGitCommits;
}
