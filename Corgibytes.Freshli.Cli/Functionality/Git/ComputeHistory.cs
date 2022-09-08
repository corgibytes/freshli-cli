using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class ComputeHistory : IComputeHistory
{
    private readonly IListCommits _listCommits;

    public ComputeHistory(IListCommits listCommits) => _listCommits = listCommits;

    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        IAnalysisLocation analysisLocation,
        string gitPath,
        string historyInterval
    )
    {
        var commitHistory = _listCommits.ForRepository(analysisLocation, gitPath);

        var groupedHistories = historyInterval switch
        {
            // Group by day of year instead of just Date otherwise the object returned it's key value is a DateTime.
            // This is different from the other checks as their key value is an integer.
            "day" => commitHistory.GroupBy(commit => commit.CommittedAt.Date.DayOfYear),

            // FirstFourDayWeek means the first week of the year that has four days before the day of the week noted.
            // So if the year starts on a Thursday and the Day of Week is set to Sunday, it'll see the next week as the first week.
            // See also https://docs.microsoft.com/en-us/dotnet/api/system.globalization.calendarweekrule?view=net-6.0
            "week" => commitHistory.GroupBy(commit =>
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(commit.CommittedAt.Date,
                    CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday)),

            // Grouped by month
            "month" => commitHistory.GroupBy(commit => commit.CommittedAt.Month),

            // Grouped by year
            "year" => commitHistory.GroupBy(commit => commit.CommittedAt.Year),
            _ => null
        };

        if (groupedHistories == null)
        {
            return new List<HistoryIntervalStop>();
        }

        // Per group, select the most recent commit.
        // So if grouped per day, pick the commit committed last on that day.
        return (
                from groupedHistory in groupedHistories
                select groupedHistory.MaxBy(i => i.CommittedAt)
                into mostRecentCommitForThisGroupedPeriod
                where mostRecentCommitForThisGroupedPeriod != null
                select new HistoryIntervalStop(mostRecentCommitForThisGroupedPeriod.ShaIdentifier,
                    mostRecentCommitForThisGroupedPeriod.CommittedAt))
            .ToList();
    }

    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(IAnalysisLocation analysisLocation, string gitPath)
    {
        var commitHistory = _listCommits.ForRepository(analysisLocation, gitPath);
        return commitHistory
            .Select(gitCommit => new HistoryIntervalStop(gitCommit.ShaIdentifier, gitCommit.CommittedAt)).ToList();
    }
}
