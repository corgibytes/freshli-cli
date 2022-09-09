using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
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
    private readonly Mock<IAnalysisLocation> _analysisLocation;

    public ComputeHistoryTest(ITestOutputHelper output) : base(output)
    {
        _listCommits = new MockListCommits();
        _computeHistory = new ComputeHistory(_listCommits, new HistoryIntervalParser());
        _analysisLocation = new Mock<IAnalysisLocation>();
    }

    [Fact]
    public void Verify_it_can_deal_with_no_commits()
    {
        _listCommits.HasCommitsAvailable(new List<GitCommit>());
        var expectedStops = new List<HistoryIntervalStop>();
        Assert.Equivalent(expectedStops, _computeHistory.ComputeWithHistoryInterval(_analysisLocation.Object, "git", "1d"));
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
                "18e3e90c27f07e734b44bb0bf9c3a8676a11f4df",
                new DateTimeOffset(2022, 1, 3, 15, 24, 24, TimeSpan.Zero)
            )
        };

        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeWithHistoryInterval(_analysisLocation.Object, "git", "1w"));
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
    [MethodData(nameof(ExpectedStopsForMonthIntervalForRealWorldScenario))]
    [MethodData(nameof(ExpectedStopsForYearIntervalForRealWorldScenario))]
    [MethodData(nameof(ExpectedStopsForTwoWeekInterval))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_interval(List<HistoryIntervalStop> expectedStops, string interval,
        List<GitCommit> availableCommits)
    {
        _listCommits.HasCommitsAvailable(availableCommits);
        Assert.Equivalent(expectedStops,
            _computeHistory.ComputeWithHistoryInterval(_analysisLocation.Object, "git", interval));
    }

    private static List<GitCommit> CommitsAvailableForTwoWeekInterval() =>
        new()
        {
            new GitCommit("be2640b3a7e098ae7b32c5444502936efc4d0d1e",
                new DateTimeOffset(2021, 5, 17, 15, 24, 24, TimeSpan.Zero)),
            new GitCommit("2737b0313671f93e7b18a93614ce1e1a3b097e02",
                new DateTimeOffset(2021, 5, 23, 01, 50, 59, TimeSpan.Zero)),
            new GitCommit("9961845aea15e9ba5f057904d1db4bcd80c74f48",
                new DateTimeOffset(2021, 5, 23, 22, 24, 24, TimeSpan.Zero)),
            new GitCommit("825a824502e04da744671501958503cf457c128f",
                new DateTimeOffset(2021, 5, 31, 15, 24, 24, TimeSpan.Zero)),
            new GitCommit("e212cb02b6df68bfbad94c71228dce944500ea82",
                new DateTimeOffset(2021, 6, 20, 15, 24, 24, TimeSpan.Zero))
        };

    private static TheoryData<List<HistoryIntervalStop>, string, List<GitCommit>> ExpectedStopsForTwoWeekInterval() =>
        new()
        {
            {
                new List<HistoryIntervalStop>
                {
                    // Third block is from 23/5 till 9/5
                    // Youngest commit in this block is on
                    new("be2640b3a7e098ae7b32c5444502936efc4d0d1e",
                        new DateTimeOffset(2021, 5, 17, 15, 24, 24, TimeSpan.Zero)),
                    // Second block is from 6/6 till 23/5
                    // Youngest commit in this block is on 31/5
                    new("825a824502e04da744671501958503cf457c128f",
                        new DateTimeOffset(2021, 5, 31, 15, 24, 24, TimeSpan.Zero)),
                    // Start point is 20/6, so our first block is from 20/6 till 6/6
                    // Closest commit is on 20/6
                    new("e212cb02b6df68bfbad94c71228dce944500ea82",
                        new DateTimeOffset(2021, 6, 20, 15, 24, 24, TimeSpan.Zero))
                },
                "2w",
                CommitsAvailableForTwoWeekInterval()
            }
        };

    private static List<GitCommit> CommitsAvailableFromRealWorldScenario() =>
        new()
        {
            new GitCommit("583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                // 2021/05/19 15:24:24
                new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)),
            new GitCommit("75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                // 2021/02/20 12:31:34
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)),
            new GitCommit("b2bd95f16a8587dd0bd618ea3415fc8928832c91",
                // 2021/02/02 13:17:05
                new DateTimeOffset(2021, 2, 2, 13, 17, 05, TimeSpan.Zero)),
            new GitCommit("57e5112ae54b7bec8a5294b7cbba2fd9bbd0a75c",
                // 2021/02/02 10:13:46
                new DateTimeOffset(2021, 2, 2, 10, 13, 46, TimeSpan.Zero)),
            new GitCommit("a4792063da2ebb7628b66b9f238cba300b18ab00",
                // 2021/02/01 19:27:42
                new DateTimeOffset(2021, 2, 1, 19, 27, 42, TimeSpan.Zero)),
            new GitCommit("9cd8467fe93714da66bce9056d527d360c6389df",
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
                "1d",
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
                "1m",
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
                "1y",
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
