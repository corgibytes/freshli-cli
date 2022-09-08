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
    private Mock<IAnalysisLocation> _analysisLocation;

    public ComputeHistoryTest(ITestOutputHelper output) : base(output)
    {
        _listCommits = new MockListCommits();
        _computeHistory = new ComputeHistory(_listCommits);
        _analysisLocation = new Mock<IAnalysisLocation>();
    }

    [Fact]
    public void Verify_it_can_deal_with_turning_of_the_year()
    {
        _listCommits.HasCommitsAvailable(new List<GitCommit>
        {
            new("583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                // 2021/12/31
                new DateTimeOffset(2021, 12, 31, 15, 24, 24, TimeSpan.Zero)),
            new("a4792063da2ebb7628b66b9f238cba300b18ab00",
                // 2022/01/01
                new DateTimeOffset(2022, 1, 1, 15, 24, 24, TimeSpan.Zero)),
            new("18e3e90c27f07e734b44bb0bf9c3a8676a11f4df",
                // 2022/01/03 first week of 2022
                new DateTimeOffset(2022, 1, 3, 15, 24, 24, TimeSpan.Zero))
        });

        var expectedStops = new List<HistoryIntervalStop>()
        {
            new(
                "a4792063da2ebb7628b66b9f238cba300b18ab00",
                new DateTimeOffset(2022, 1, 1, 15, 24, 24, TimeSpan.Zero)
            ),
            new(
                "18e3e90c27f07e734b44bb0bf9c3a8676a11f4df",
                new DateTimeOffset(2022, 1, 3, 15, 24, 24, TimeSpan.Zero)
            )
        };

        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeWithHistoryInterval(_analysisLocation.Object, "git", "week"));
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
        Assert.Equivalent(new List<HistoryIntervalStop>(),
            _computeHistory.ComputeWithHistoryInterval(_analysisLocation.Object, "git", "icannotbelievethisisrealhistoryinterval"));
    }

    [Theory]
    [MethodData(nameof(ExpectedStopsForCommitHistoryForRealWorldScenario))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_the_all_commits(
        List<HistoryIntervalStop> expectedStops, List<GitCommit> availableCommits)
    {
        _listCommits.HasCommitsAvailable(availableCommits);
        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeCommitHistory(_analysisLocation.Object, "git")
        );
    }

    [Theory]
    [MethodData(nameof(ExpectedStopsForDayIntervalForRealWorldScenario))]
    [MethodData(nameof(ExpectedStopsForWeekIntervalForRealWorldScenario))]
    [MethodData(nameof(ExpectedStopsForMonthIntervalForRealWorldScenario))]
    [MethodData(nameof(ExpectedStopsForYearIntervalForRealWorldScenario))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_interval(List<HistoryIntervalStop> expectedStops, string interval,
        List<GitCommit> availableCommits)
    {
        _listCommits.HasCommitsAvailable(availableCommits);
        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeWithHistoryInterval(_analysisLocation.Object, "git", interval));
    }

    private static void CommitsAvailableCrossingYears(MockListCommits listCommits) =>
        listCommits.HasCommitsAvailable(new List<GitCommit>
        {
            new("583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                // 2021/12/19 15:24:24
                new DateTimeOffset(2021, 12, 19, 15, 24, 24, TimeSpan.Zero)),
            new("75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                // 2021/12/20 12:31:34
                new DateTimeOffset(2021, 12, 20, 12, 31, 34, TimeSpan.Zero)),
            new("9cd8467fe93714da66bce9056d527d360c6389df",
                // 2022/01/01 19:26:16
                new DateTimeOffset(2021, 2, 1, 19, 26, 16, TimeSpan.Zero)),
            new("a4792063da2ebb7628b66b9f238cba300b18ab00",
                // 2022/01/01 19:27:42
                new DateTimeOffset(2022, 1, 1, 19, 27, 42, TimeSpan.Zero)),
            new("57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c",
                // 2022/01/02 10:13:46
                new DateTimeOffset(2022, 1, 2, 10, 13, 46, TimeSpan.Zero)),
            new("b2bd95f16a8587dd0bd618ea3415fc8928832c91",
                // 2022/02/02 13:17:05
                new DateTimeOffset(2022, 2, 2, 13, 17, 05, TimeSpan.Zero))
        });

    private static List<GitCommit> CommitsAvailableFromRealWorldScenario() =>
        new()
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
        };

    private static TheoryData<List<HistoryIntervalStop>, string, List<GitCommit>> ExpectedStopsForDayIntervalForRealWorldScenario() =>
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
                "day",
                CommitsAvailableFromRealWorldScenario()
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string, List<GitCommit>> ExpectedStopsForWeekIntervalForRealWorldScenario() =>
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
                "week",
                CommitsAvailableFromRealWorldScenario()
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string, List<GitCommit>> ExpectedStopsForMonthIntervalForRealWorldScenario() =>
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
                "month",
                CommitsAvailableFromRealWorldScenario()
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, string, List<GitCommit>> ExpectedStopsForYearIntervalForRealWorldScenario() =>
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
                "year",
                CommitsAvailableFromRealWorldScenario()
            }
        };

    private static TheoryData<List<HistoryIntervalStop>, List<GitCommit>> ExpectedStopsForCommitHistoryForRealWorldScenario() =>
        new()
        {
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
                },
                CommitsAvailableFromRealWorldScenario()
            }
        };
}
