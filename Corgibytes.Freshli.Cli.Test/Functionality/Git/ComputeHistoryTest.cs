using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Test.Common;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class ComputeHistoryTest : FreshliTest
{
    private readonly ComputeHistory _computeHistory;

    public ComputeHistoryTest(ITestOutputHelper output) : base(output)
    {
        MockListCommits listCommits = new();
        _computeHistory = new(listCommits);

        listCommits.HasCommitsAvailable(new List<GitCommit>
        {
            new("583d813db3e28b9b44a29db352e2f0e1b4c6e420", new(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)),
            new("75c7fcc7336ee718050c4a5c8dfb5598622787b2", new(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)),
            new("b2bd95f16a8587dd0bd618ea3415fc8928832c91", new(2021, 2, 2, 13, 17, 05, TimeSpan.Zero)),
            new("57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c", new(2021, 2, 2, 10, 13, 46, TimeSpan.Zero)),
            new("a4792063da2ebb7628b66b9f238cba300b18ab00", new(2021, 2, 1, 19, 27, 42, TimeSpan.Zero)),
            new("9cd8467fe93714da66bce9056d527d360c6389df", new(2021, 2, 1, 19, 26, 16, TimeSpan.Zero))
        });
    }

    [Theory]
    [MethodData(nameof(ExpectedStopsForDayInterval))]
    [MethodData(nameof(ExpectedStopsForWeekInterval))]
    [MethodData(nameof(ExpectedStopsForMonthInterval))]
    [MethodData(nameof(ExpectedStopsForYearInterval))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_interval(List<HistoryIntervalStop> expectedStops,
        string interval) => Assert.Equivalent(expectedStops,
        _computeHistory.ComputeWithHistoryInterval("repository.identifier", "git", interval));

    [Theory]
    [MethodData(nameof(ExpectedStopsForCommitHistory))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_the_all_commits(
        List<HistoryIntervalStop> expectedStops) =>
        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeCommitHistory("repository.identifier", "git")
        );

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForDayInterval() =>
        new()
        {
            {
                new()
                {
                    new(
                        new(2021, 2, 1, 19, 27, 42, TimeSpan.Zero),
                        "a4792063da2ebb7628b66b9f238cba300b18ab00"
                    ),
                    new(
                        new(2021, 2, 2, 13, 17, 05, TimeSpan.Zero),
                        "b2bd95f16a8587dd0bd618ea3415fc8928832c91"
                    ),
                    new(
                        new(2021, 2, 20, 12, 31, 34, TimeSpan.Zero),
                        "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                    ),
                    new(
                        new(2021, 5, 19, 15, 24, 24, TimeSpan.Zero),
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
                    )
                },
                "day"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForWeekInterval() =>
        new()
        {
            {
                new()
                {
                    new(
                        new(2021, 2, 2, 13, 17, 05, TimeSpan.Zero),
                        "b2bd95f16a8587dd0bd618ea3415fc8928832c91"
                    ),
                    new(
                        new(2021, 2, 20, 12, 31, 34, TimeSpan.Zero),
                        "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                    ),
                    new(
                        new(2021, 5, 19, 15, 24, 24, TimeSpan.Zero),
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
                    )
                },
                "week"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForMonthInterval() =>
        new()
        {
            {
                new()
                {
                    new(
                        new(2021, 2, 20, 12, 31, 34, TimeSpan.Zero),
                        "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                    ),
                    new(
                        new(2021, 5, 19, 15, 24, 24, TimeSpan.Zero),
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
                    )
                },
                "month"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForYearInterval() =>
        new()
        {
            {
                new()
                {
                    new(
                        new(2021, 5, 19, 15, 24, 24, TimeSpan.Zero),
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
                    )
                },
                "year"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>> ExpectedStopsForCommitHistory() =>
        new()
        {
            new()
            {
                new(
                    new(2021, 2, 1, 19, 26, 16, TimeSpan.Zero),
                    "9cd8467fe93714da66bce9056d527d360c6389df"
                ),
                new(
                    new(2021, 2, 1, 19, 27, 42, TimeSpan.Zero),
                    "a4792063da2ebb7628b66b9f238cba300b18ab00"
                ),
                new(
                    new(2021, 2, 2, 10, 13, 46, TimeSpan.Zero),
                    "57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c"
                ),
                new(
                    new(2021, 2, 2, 13, 17, 05, TimeSpan.Zero),
                    "b2bd95f16a8587dd0bd618ea3415fc8928832c91"
                ),
                new(
                    new(2021, 2, 20, 12, 31, 34, TimeSpan.Zero),
                    "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                ),
                new(
                    new(2021, 5, 19, 15, 24, 24, TimeSpan.Zero),
                    "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
                )
            }
        };
}
