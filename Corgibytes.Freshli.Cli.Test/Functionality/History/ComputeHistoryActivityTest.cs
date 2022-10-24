using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Test.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

public class ComputeHistoryActivityTest
{
    private const int HistoryStopPointId = 29;
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IComputeHistory> _computeHistory = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    public ComputeHistoryActivityTest()
    {
        Configuration = new Configuration(new MockEnvironment());

        _cacheManager.Setup(mock => mock.GetCacheDb()).Returns(_cacheDb.Object);

        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(Configuration);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(_computeHistory.Object);

        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);

        HistoryStopData =
            new HistoryStopData(Configuration, "test", "abcde1234",
                new DateTimeOffset(2022, 9, 1, 1, 0, 0, TimeSpan.Zero));
    }

    private HistoryStopData HistoryStopData { get; }
    private Configuration Configuration { get; }

    [Fact]
    public void FiresHistoryIntervalStopFoundEvents()
    {
        SetupCachedAnalysis("https://lorem-ipsum.com", "main", "1m", CommitHistory.AtInterval,
            RevisionHistoryMode.AllRevisions);

        // Have interval stops available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            ),
            new(
                "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
            )
        };
        _computeHistory.Setup(mock => mock.ComputeWithHistoryInterval(
                It.IsAny<IHistoryStopData>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        new ComputeHistoryActivity(
            analysisId,
            HistoryStopData
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisId == analysisId &&
                        value.HistoryStopPointId == HistoryStopPointId
                )
            )
        );
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisId == analysisId &&
                        value.HistoryStopPointId == HistoryStopPointId
                )
            )
        );
    }

    [Fact]
    public void FiresHistoryIntervalStopFoundEventsForComputeHistory()
    {
        SetupCachedAnalysis("https://lorem-ipsum.com", "main", "1m", CommitHistory.Full,
            RevisionHistoryMode.AllRevisions);

        // Have interval stops available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            )
        };
        _computeHistory.Setup(mock => mock.ComputeCommitHistory(
                It.IsAny<IHistoryStopData>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        new ComputeHistoryActivity(
            analysisId,
            HistoryStopData
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisId == analysisId &&
                        value.HistoryStopPointId == HistoryStopPointId
                )
            )
        );
    }

    [Fact]
    public void FiresHistoryIntervalStopFoundEventsForLatestOnly()
    {
        SetupCachedAnalysis("https://lorem-ipsum.com", "main", "1m", CommitHistory.Full,
            RevisionHistoryMode.OnlyLatestRevision);

        // Have interval stop available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            )
        };
        _computeHistory.Setup(mock => mock.ComputeLatestOnly(
                It.IsAny<IHistoryStopData>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        new ComputeHistoryActivity(
            analysisId,
            HistoryStopData
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisId == analysisId &&
                        value.HistoryStopPointId == HistoryStopPointId
                )
            )
        );
    }

    [Fact]
    public void FiresInvalidHistoryIntervalStopEvent()
    {
        // This could happen when we run the analysis on a codebase that barely has any commits.
        // If we want to analyse it, we have to be wary of the interval not being bigger than the age of the first commit.
        // e.g. if it's less than a year old, running the analysis with a 1y interval breaks.

        SetupCachedAnalysis("https://lorem-ipsum.com", "main", "1y", CommitHistory.AtInterval,
            RevisionHistoryMode.AllRevisions);

        var listCommits = new MockListCommits();
        listCommits.HasCommitsAvailable(new List<GitCommit>
        {
            new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                new DateTimeOffset(2022, 1, 29, 00, 00, 00, TimeSpan.Zero)),
            new("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                new DateTimeOffset(2022, 1, 30, 00, 00, 00, TimeSpan.Zero))
        });

        var computeHistory = new ComputeHistory(listCommits, new HistoryIntervalParser());

        _serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(computeHistory);

        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        new ComputeHistoryActivity(analysisId, HistoryStopData).Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<InvalidHistoryIntervalEvent>(value =>
                value.ErrorMessage ==
                "Given range (1y) results in an invalid start date as it occurs before date of oldest commit")
            ));
    }

    private void SetupCachedAnalysis(string repositoryUrl, string repositoryBranch, string historyInterval,
        CommitHistory useCommitHistory, RevisionHistoryMode revisionHistoryMode)
    {
        // Arrange
        // Have an analysis available
        var cachedAnalysis = new CachedAnalysis(repositoryUrl, repositoryBranch, historyInterval, useCommitHistory,
            revisionHistoryMode);
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(It.IsAny<Guid>())).Returns(cachedAnalysis);
        _cacheDb.Setup(mock => mock.AddHistoryStopPoint(It.IsAny<CachedHistoryStopPoint>()))
            .Returns(HistoryStopPointId);
    }
}
