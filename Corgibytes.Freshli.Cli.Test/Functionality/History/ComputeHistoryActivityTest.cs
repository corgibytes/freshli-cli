using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

public class ComputeHistoryActivityTest
{
    private readonly Mock<IAnalysisLocation> _analysisLocation = new();
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IComputeHistory> _computeHistory = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    public ComputeHistoryActivityTest()
    {
        _cacheManager.Setup(mock => mock.GetCacheDb()).Returns(_cacheDb.Object);

        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(_configuration.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(_computeHistory.Object);

        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

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
                It.IsAny<IAnalysisLocation>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        new ComputeHistoryActivity(
            analysisId,
            _analysisLocation.Object
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisId == analysisId &&
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                )
            )
        );
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
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
                It.IsAny<IAnalysisLocation>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        new ComputeHistoryActivity(
            analysisId,
            _analysisLocation.Object
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisId == analysisId &&
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
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
                It.IsAny<IAnalysisLocation>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        new ComputeHistoryActivity(
            analysisId,
            _analysisLocation.Object
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.AnalysisId == analysisId &&
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                )
            )
        );
    }

    private void SetupCachedAnalysis(string repositoryUrl, string repositoryBranch, string historyInterval,
        CommitHistory useCommitHistory, RevisionHistoryMode revisionHistoryMode)
    {
        // Arrange
        // Have an analysis available
        var cachedAnalysis = new CachedAnalysis(repositoryUrl, repositoryBranch, historyInterval, useCommitHistory,
            revisionHistoryMode);
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(It.IsAny<Guid>())).Returns(cachedAnalysis);
    }
}
