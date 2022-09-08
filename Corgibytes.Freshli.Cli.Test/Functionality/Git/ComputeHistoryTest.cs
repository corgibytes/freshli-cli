using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Test.Common;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[UnitTest]
public class ComputeHistoryTest : FreshliTest
{
    private readonly ComputeHistory _computeHistory;
    private readonly MockListCommits _listCommits;

    public ComputeHistoryTest(ITestOutputHelper output) : base(output)
    {
        _listCommits = new MockListCommits();
        _computeHistory = new ComputeHistory(_listCommits);
    }

    [Fact]
    public void Verify_it_returns_an_empty_list_when_an_invalid_interval_is_used()
    {
        _listCommits.HasCommitsAvailable(new List<GitCommit>
        {
            new("583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                // 2021/05/19 15:24:24
                new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero))
        });
        var analysisLocation = new Mock<IAnalysisLocation>();
        Assert.Equivalent(new List<HistoryIntervalStop>(), _computeHistory.ComputeWithHistoryInterval(analysisLocation.Object, "git", "icannotbelievethisisrealhistoryinterval"));
    }

    [Theory]
    [MethodData(nameof(ExpectedStopsForCommitHistory))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_the_all_commits(
        List<HistoryIntervalStop> expectedStops)
    {
        CommitsAvailableFromRealWorldScenario(_listCommits);
        var analysisLocation = new Mock<IAnalysisLocation>();

        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeCommitHistory(analysisLocation.Object, "git")
        );
    }

    [Theory]
    [MethodData(nameof(ExpectedStopsForDayInterval))]
    [MethodData(nameof(ExpectedStopsForWeekInterval))]
    [MethodData(nameof(ExpectedStopsForMonthInterval))]
    [MethodData(nameof(ExpectedStopsForYearInterval))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_interval(List<HistoryIntervalStop> expectedStops,
        string interval)
    {
        CommitsAvailableFromRealWorldScenario(_listCommits);
        var analysisLocation = new Mock<IAnalysisLocation>();
        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeWithHistoryInterval(analysisLocation.Object, "git", interval));
    }

    private static void CommitsAvailableFromRealWorldScenario(MockListCommits listCommits) =>
        listCommits.HasCommitsAvailable(new List<GitCommit>
        {
            new("583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                // 2021/05/19 15:24:24
                new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)),
            new("75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                // 2021/02/20 12:31:34
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)),
            new("b2bd95f16a8587dd0bd618ea3415fc8928832c91",
                // 2021/02/02 13:17:05
                new DateTimeOffset(2021, 2, 2, 13, 17, 05, TimeSpan.Zero)),
            new("57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c",
                // 2021/02/02 10:13:46
                new DateTimeOffset(2021, 2, 2, 10, 13, 46, TimeSpan.Zero)),
            new("a4792063da2ebb7628b66b9f238cba300b18ab00",
                // 2021/02/01 19:27:42
                new DateTimeOffset(2021, 2, 1, 19, 27, 42, TimeSpan.Zero)),
            new("9cd8467fe93714da66bce9056d527d360c6389df",
                // 2021/02/01 19:26:16
                new DateTimeOffset(2021, 2, 1, 19, 26, 16, TimeSpan.Zero))
        });

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForDayInterval() =>
        new()
        {
            {
                new List<HistoryIntervalStop>
                {
                    new(
                        "a4792063da2ebb7628b66b9f238cba300b18ab00",
                        new DateTimeOffset(2021, 2, 1, 19, 27, 42, TimeSpan.Zero)
                    ),
                    new(
                        "b2bd95f16a8587dd0bd618ea3415fc8928832c91",
                        new DateTimeOffset(2021, 2, 2, 13, 17, 05, TimeSpan.Zero)
                    ),
                    new(
                        "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                        new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
                    ),
                    new(
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                        new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
                    )
                },
                "day"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForWeekInterval() =>
        new()
        {
            {
                new List<HistoryIntervalStop>
                {
                    new(
                        "b2bd95f16a8587dd0bd618ea3415fc8928832c91",
                        new DateTimeOffset(2021, 2, 2, 13, 17, 05, TimeSpan.Zero)
                    ),
                    new(
                        "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                        new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
                    ),
                    new(
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                        new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
                    )
                },
                "week"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForMonthInterval() =>
        new()
        {
            {
                new List<HistoryIntervalStop>
                {
                    new(
                        "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                        new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
                    ),
                    new(
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                        new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
                    )
                },
                "month"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string> ExpectedStopsForYearInterval() =>
        new()
        {
            {
                new List<HistoryIntervalStop>
                {
                    new(
                        "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                        new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
                    )
                },
                "year"
            }
        };

    private static TheoryData<List<HistoryIntervalStop>> ExpectedStopsForCommitHistory() =>
        new()
        {
            new List<HistoryIntervalStop>
            {
                new(
                    "9cd8467fe93714da66bce9056d527d360c6389df",
                    new DateTimeOffset(2021, 2, 1, 19, 26, 16, TimeSpan.Zero)
                ),
                new(
                    "a4792063da2ebb7628b66b9f238cba300b18ab00",
                    new DateTimeOffset(2021, 2, 1, 19, 27, 42, TimeSpan.Zero)
                ),
                new(
                    "57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c",
                    new DateTimeOffset(2021, 2, 2, 10, 13, 46, TimeSpan.Zero)
                ),
                new(
                    "b2bd95f16a8587dd0bd618ea3415fc8928832c91",
                    new DateTimeOffset(2021, 2, 2, 13, 17, 05, TimeSpan.Zero)
                ),
                new(
                    "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                    new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
                ),
                new(
                    "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                    new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
                )
            }
        };
}
