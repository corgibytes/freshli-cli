using System;
using System.Collections.Generic;
using System.Linq;
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
        Assert.Equivalent(expectedStops, _computeHistory.ComputeWithHistoryInterval(_analysisLocation.Object, "git", "1d", DateTimeOffset.Now));
    }

    [Fact]
    public void Verify_it_can_list_all_commits()
    {
        _listCommits.HasCommitsAvailable(AvailableCommits());
        var expectedStops = new List<HistoryIntervalStop>()
        {
            new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                new DateTimeOffset(2021, 1, 29, 00, 00, 00, TimeSpan.Zero)),
            new("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                new DateTimeOffset(2021, 1, 12, 00, 00, 00, TimeSpan.Zero)),
            new("ef14791d014431952aa721fa2a9b22afb8d4f144",
                new DateTimeOffset(2021, 1, 13, 00, 00, 00, TimeSpan.Zero)),
            new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                new DateTimeOffset(2020, 12, 31, 00, 00, 00, TimeSpan.Zero))
        };

        var actualStops = _computeHistory.ComputeCommitHistory(_analysisLocation.Object, "git").ToList();

        Assert.NotStrictEqual(expectedStops, actualStops);
        Assert.Equal(expectedStops.Count, actualStops.Count);
    }

    [Theory]
    [MethodData(nameof(DataForTwoWeekInterval))]
    [MethodData(nameof(DataForOneDayInterval))]
    [MethodData(nameof(DataForOneWeekInterval))]
    [MethodData(nameof(DataForOneMonthInterval))]
    [MethodData(nameof(DataForOneYearInterval))]
    public void Verify_it_can_find_sha_identifiers_and_dates_for_interval(string interval, DateTimeOffset startAtDate,
        List<GitCommit> availableCommits, List<HistoryIntervalStop> expectedStops)
    {
        _listCommits.HasCommitsAvailable(availableCommits);
        var actualStops = _computeHistory.ComputeWithHistoryInterval(
            _analysisLocation.Object,
            "git",
            interval,
            startAtDate
        ).ToList();
        Assert.NotStrictEqual(expectedStops, actualStops);
        Assert.Equal(expectedStops.Count, actualStops.Count);
    }

    private static List<GitCommit> AvailableCommits() =>
        new()
        {
            // Friday week 4
            new GitCommit("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                new DateTimeOffset(2021, 1, 29, 00, 00, 00, TimeSpan.Zero)),
            // Tuesday week 2
            new GitCommit("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                new DateTimeOffset(2021, 1, 12, 00, 00, 00, TimeSpan.Zero)),
            // Wednesday week 2
            new GitCommit("ef14791d014431952aa721fa2a9b22afb8d4f144",
                new DateTimeOffset(2021, 1, 13, 00, 00, 00, TimeSpan.Zero)),
            // Thursday week 53
            new GitCommit("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                new DateTimeOffset(2020, 12, 31, 00, 00, 00, TimeSpan.Zero))
        };

    public static TheoryData<string, DateTimeOffset, List<GitCommit>, List<HistoryIntervalStop>> DataForTwoWeekInterval() =>
        new()
        {
            {
                "2w",
                new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero),
                AvailableCommits(),
                new List<HistoryIntervalStop>
                {
                    // Friday week 4
                    new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                        new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero)),
                    // Monday week 4 (start of range)
                    new("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                        new DateTimeOffset(2021, 1, 25, 00, 00, 00, TimeSpan.Zero)),
                    // Monday week 2
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2021, 1, 11, 00, 00, 00, TimeSpan.Zero)),
                    // Thursday week 53
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2020, 12, 31, 00, 00, 00, TimeSpan.Zero))
                }
            }
        };

    public static TheoryData<string, DateTimeOffset, List<GitCommit>, List<HistoryIntervalStop>> DataForOneDayInterval() =>
        new()
        {
            {
                "1d",
                new DateTimeOffset(2021, 1, 5, 00, 00, 00, TimeSpan.Zero),
                new List<GitCommit>
                {
                    new("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                        new DateTimeOffset(2021, 1, 4, 00, 00, 00, TimeSpan.Zero)),
                    new("ef14791d014431952aa721fa2a9b22afb8d4f144",
                        new DateTimeOffset(2021, 1, 3, 00, 00, 00, TimeSpan.Zero)),
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2020, 12, 31, 00, 00, 00, TimeSpan.Zero))
                },
                new List<HistoryIntervalStop>
                {
                    // Start date
                    new("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                        new DateTimeOffset(2021, 1, 5, 00, 00, 00, TimeSpan.Zero)),
                    new("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                        new DateTimeOffset(2021, 1, 4, 00, 00, 00, TimeSpan.Zero)),
                    new("ef14791d014431952aa721fa2a9b22afb8d4f144",
                        new DateTimeOffset(2021, 1, 3, 00, 00, 00, TimeSpan.Zero)),
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2021, 1, 2, 00, 00, 00, TimeSpan.Zero)),
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2021, 1, 1, 00, 00, 00, TimeSpan.Zero)),
                    // End date
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2020, 12, 31, 00, 00, 00, TimeSpan.Zero))
                }
            }
        };

    public static TheoryData<string, DateTimeOffset, List<GitCommit>, List<HistoryIntervalStop>> DataForOneWeekInterval() =>
        new()
        {
            {
                "1w",
                new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero),
                AvailableCommits(),
                new List<HistoryIntervalStop>
                {
                    // Friday week 4
                    new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                        new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero)),
                    // Monday week 4
                    new("ef14791d014431952aa721fa2a9b22afb8d4f144",
                        new DateTimeOffset(2021, 1, 25, 00, 00, 00, TimeSpan.Zero)),
                    // Monday week 3
                    new("ef14791d014431952aa721fa2a9b22afb8d4f144",
                        new DateTimeOffset(2021, 1, 18, 00, 00, 00, TimeSpan.Zero)),
                    // Monday week 2
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2021, 1, 11, 00, 00, 00, TimeSpan.Zero)),
                    // Monday week 1
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2021, 1, 4, 00, 00, 00, TimeSpan.Zero)),
                    // Thursday week 53
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2020, 12, 31, 00, 00, 00, TimeSpan.Zero))
                }
            }
        };

    public static TheoryData<string, DateTimeOffset, List<GitCommit>, List<HistoryIntervalStop>> DataForOneMonthInterval() =>
        new()
        {
            {
                "1m",
                new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero),
                AvailableCommits(),
                new List<HistoryIntervalStop>
                {
                    // Start date
                    new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                        new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero)),
                    // First day of first month
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2021, 1, 1, 00, 00, 00, TimeSpan.Zero)),
                    // End date
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2020, 12, 31, 00, 00, 00, TimeSpan.Zero))
                }
            }
        };

    public static TheoryData<string, DateTimeOffset, List<GitCommit>, List<HistoryIntervalStop>> DataForOneYearInterval() =>
        new()
        {
            {
                "1y",
                new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero),
                new List<GitCommit>()
                {
                    // Friday week 4
                    new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                        new DateTimeOffset(2021, 1, 29, 00, 00, 00, TimeSpan.Zero)),
                    // Thursday week 53
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2019, 12, 31, 00, 00, 00, TimeSpan.Zero))
                },
                new List<HistoryIntervalStop>
                {
                    // Start date
                    new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                        new DateTimeOffset(2021, 1, 31, 00, 00, 00, TimeSpan.Zero)),
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2021, 1, 1, 00, 00, 00, TimeSpan.Zero)),
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2020, 1, 1, 00, 00, 00, TimeSpan.Zero)),
                    // End date
                    new("4f6b7990ad45b2c5bf5817c359de72729654dd9f",
                        new DateTimeOffset(2019, 12, 31, 00, 00, 00, TimeSpan.Zero))
                }
            }
        };
}
