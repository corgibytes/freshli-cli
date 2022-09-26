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
        string historyInterval,
        DateTimeOffset startAtDate
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

        // Here be dragons!
        // The code that follows looks odd but it's important to remember we are walking back in time.
        var range = new List<DateTimeOffset>
        {
            startAtDate
        };

        var rangeStartDate = DetermineRangeStartDate(startAtDate, quantifier);

        if (rangeStartDate != startAtDate)
        {
            range.Add(rangeStartDate);
        }

        while (rangeStartDate > oldestCommit.CommittedAt)
        {
            var dateTimeOffset = quantifier switch
            {
                "d" =>
                    // Go back at interval -x days
                    rangeStartDate.AddDays(-interval),
                "w" =>
                    // Go back at interval of -7 * interval days
                    rangeStartDate.AddDays(-7 * interval),
                "m" =>
                    // Go back at interval of -x months
                    rangeStartDate.AddMonths(-interval),
                "y" =>
                    // Go back at interval of -x years
                    rangeStartDate.AddYears(-interval),
                _ => default
            };

            if (dateTimeOffset < oldestCommit.CommittedAt)
            {
                range.Add(oldestCommit.CommittedAt);
                break;
            }

            range.Add(dateTimeOffset);
            rangeStartDate = dateTimeOffset;
        }

        // Foreach offset in range, select the youngest commit, as long as it's not younger than the offset.
        return (
            from offset in range
            let lastCommitForOffset = gitCommits.First(commit => commit.CommittedAt <= offset)
            select new HistoryIntervalStop(lastCommitForOffset.ShaIdentifier, offset))
        .ToList();
    }

    private static DateTimeOffset DetermineRangeStartDate(DateTimeOffset startAtDate, string? quantifier)
    {
        var rangeStartDate = startAtDate;
        switch (quantifier)
        {
            case "w":
                {
                    // Start at first monday
                    var daysToSubtract = (7 + (int)rangeStartDate.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                    rangeStartDate = rangeStartDate.AddDays(-daysToSubtract);
                    break;
                }
            case "m":
                // Start at first day of the month
                rangeStartDate = new DateTimeOffset(rangeStartDate.Year, rangeStartDate.Month, 1, 0, 0, 0, rangeStartDate.Offset);
                break;
            case "y":
                // Start at first day of the year
                rangeStartDate = new DateTimeOffset(rangeStartDate.Year, 1, 1, 0, 0, 0, rangeStartDate.Offset);
                break;
        }

        return rangeStartDate;
    }

    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(IAnalysisLocation analysisLocation, string gitPath)
    {
        var commitHistory = _listCommits.ForRepository(analysisLocation, gitPath);
        return commitHistory
            .Select(gitCommit => new HistoryIntervalStop(gitCommit.ShaIdentifier, gitCommit.CommittedAt)).ToList();
    }

    public IEnumerable<HistoryIntervalStop> ComputeLatestOnly(IAnalysisLocation analysisLocation, string gitPath)
    {
        var gitCommit = _listCommits.MostRecentCommit(analysisLocation, gitPath);
        return new List<HistoryIntervalStop>{new(gitCommit.ShaIdentifier, gitCommit.CommittedAt)};
    }
}
