using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class ComputeHistory : IComputeHistory
{
    private readonly IListCommits _listCommits;
    private readonly IHistoryIntervalParser _historyIntervalParser;

    public ComputeHistory(IListCommits listCommits, IHistoryIntervalParser historyIntervalParser)
    {
        _listCommits = listCommits;
        _historyIntervalParser = historyIntervalParser;
    }

    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        IAnalysisLocation analysisLocation,
        string gitPath,
        string historyInterval
    )
    {
        _historyIntervalParser.Parse(historyInterval, out var interval, out var quantifier);

        var commitHistory = _listCommits.ForRepository(analysisLocation, gitPath);

        // Prevent multiple enumeration
        var gitCommits = commitHistory.OrderByDescending(commit => commit.CommittedAt).ToList();
        if (gitCommits.Count == 0)
        {
            return new List<HistoryIntervalStop>();
        }

        // Technically this could return null, but not in our case since we already know we have some commits
        var oldestCommit = gitCommits.MinBy(commit => commit.CommittedAt)!;
        var latestCommit = gitCommits.MaxBy(commit => commit.CommittedAt)!;

        // Here be dragons!
        // The code that follows looks odd but it's important to remember we are walking back in time.
        var startDate = new DateTimeOffset(
            latestCommit.CommittedAt.Year,
            latestCommit.CommittedAt.Month,
            latestCommit.CommittedAt.Day,
            0,
            0,
            0,
            latestCommit.CommittedAt.Offset
        );
        var range = new List<DateTimeOffset>();
        while (startDate > oldestCommit.CommittedAt)
        {
            var dateTimeOffset = quantifier switch
            {
                "d" =>
                    // Go back at interval -x days
                    startDate.AddDays(-interval),
                "w" =>
                    // Go back at interval of -7 * interval days
                    startDate.AddDays(-7 * interval),
                "m" =>
                    // Go back at interval of -x months
                    startDate.AddMonths(-interval),
                "y" =>
                    // Go back at interval of -x years
                    startDate.AddYears(-interval),
                _ => default
            };

            range.Add(dateTimeOffset);
            startDate = dateTimeOffset;
        }

        var filteredCommits = new List<HistoryIntervalStop>();
        var previousOffset = latestCommit.CommittedAt;

        foreach (var offset in range)
        {
            try
            {
                // Pick the youngest commit closest to, but not older than, the offset or younger than the previous offset
                var lastCommitForOffset = gitCommits.First(commit => commit.CommittedAt > offset && commit.CommittedAt <= previousOffset);
                filteredCommits.Add(new HistoryIntervalStop(lastCommitForOffset.ShaIdentifier, lastCommitForOffset.CommittedAt));
            }
            catch (InvalidOperationException)
            {
                // We ignore the exception as it could be that there's no commit for these dates
            }
            previousOffset = offset;
        }

        return filteredCommits;
    }

    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(IAnalysisLocation analysisLocation, string gitPath)
    {
        var commitHistory = _listCommits.ForRepository(analysisLocation, gitPath);
        return commitHistory
            .Select(gitCommit => new HistoryIntervalStop(gitCommit.ShaIdentifier, gitCommit.CommittedAt)).ToList();
    }
}
