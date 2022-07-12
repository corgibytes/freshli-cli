using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class ComputeHistory
{
    private readonly IGitCommitRepository _gitCommitRepository;

    public ComputeHistory(IGitCommitRepository gitCommitRepository) => _gitCommitRepository = gitCommitRepository;

    public IEnumerable<HistoryIntervalStop> ComputeCommitHistory(string repositoryId, string gitPath)
    {
        var commitHistory = _gitCommitRepository.ListCommits(repositoryId, gitPath);
        return commitHistory
            .Select(gitCommit => new HistoryIntervalStop(gitCommit.CommittedAt, gitCommit.ShaIdentifier)).ToList();
    }

    public IEnumerable<HistoryIntervalStop> ComputeWithHistoryInterval(
        string repositoryId,
        string gitPath,
        string historyInterval
    )
    {
        var commitHistory = _gitCommitRepository.ListCommits(repositoryId, gitPath);

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

        // Per group, select the most recent commit.
        // So if grouped per day, pick the commit committed last on that day.
        return (
                from groupedHistory in groupedHistories
                select groupedHistory.MaxBy(i => i.CommittedAt)
                into mostRecentCommitForThisDay
                where mostRecentCommitForThisDay != null
                select new HistoryIntervalStop(mostRecentCommitForThisDay.CommittedAt,
                    mostRecentCommitForThisDay.ShaIdentifier))
            .ToList();
    }
}
