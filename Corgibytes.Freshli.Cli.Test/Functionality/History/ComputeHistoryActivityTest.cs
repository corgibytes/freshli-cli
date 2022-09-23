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
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<IComputeHistory> _computeHistory = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();

    [Fact]
    public void FiresHistoryIntervalStopFoundEvents()
    {
        // Arrange
        // Have an analysis available
        var cachedAnalysis = new CachedAnalysis("https://lorem-ipsum.com", "main", "1m", CommitHistory.AtInterval, LatestOnly.WalkBackInRevisionHistory);
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(It.IsAny<Guid>())).Returns(cachedAnalysis);

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
                It.IsAny<IAnalysisLocation>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>())
            )
            .Returns(historyIntervalStops);

        var analysisLocation = new Mock<IAnalysisLocation>();

        var serviceProvider = new Mock<IServiceProvider>();
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var cacheManager = new Mock<ICacheManager>();
        cacheManager.Setup(mock => mock.GetCacheDb(It.IsAny<string>())).Returns(_cacheDb.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(_computeHistory.Object);

        // Act
        new ComputeHistoryActivity(
            "git",
            new Guid("cbc83480-ae47-46de-91df-60747ca8fb09"),
            analysisLocation.Object
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.GitExecutablePath == "git" &&
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                )
            )
        );
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.GitExecutablePath == "git" &&
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "583d813db3e28b9b44a29db352e2f0e1b4c6e420"
                )
            )
        );
    }

    [Fact]
    public void FiresHistoryIntervalStopFoundEventsForComputeHistory()
    {
        // Arrange
        // Have an analysis available
        var cachedAnalysis = new CachedAnalysis("https://lorem-ipsum.com", "main", "1m", CommitHistory.Full, LatestOnly.WalkBackInRevisionHistory);
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(It.IsAny<Guid>())).Returns(cachedAnalysis);

        // Have interval stops available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            )
        };
        _computeHistory.Setup(mock => mock.ComputeCommitHistory(
                It.IsAny<IAnalysisLocation>(), It.IsAny<string>())
            )
            .Returns(historyIntervalStops);

        var analysisLocation = new Mock<IAnalysisLocation>();

        var serviceProvider = new Mock<IServiceProvider>();
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var cacheManager = new Mock<ICacheManager>();
        cacheManager.Setup(mock => mock.GetCacheDb(It.IsAny<string>())).Returns(_cacheDb.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(_computeHistory.Object);

        // Act
        new ComputeHistoryActivity(
            "git",
            new Guid("cbc83480-ae47-46de-91df-60747ca8fb09"),
            analysisLocation.Object
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.GitExecutablePath == "git" &&
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                )
            )
        );
    }

    [Fact]
    public void FiresHistoryIntervalStopFoundEventsForLatestOnly()
    {
        // Arrange
        // Have an analysis available
        var cachedAnalysis = new CachedAnalysis("https://lorem-ipsum.com", "main", "1m", CommitHistory.Full, LatestOnly.ShowLatestOnly);
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(It.IsAny<Guid>())).Returns(cachedAnalysis);

        // Have interval stop available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            )
        };
        _computeHistory.Setup(mock => mock.ComputeLatestOnly(
                It.IsAny<IAnalysisLocation>(), It.IsAny<string>())
            )
            .Returns(historyIntervalStops);

        var analysisLocation = new Mock<IAnalysisLocation>();

        var serviceProvider = new Mock<IServiceProvider>();
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var cacheManager = new Mock<ICacheManager>();
        cacheManager.Setup(mock => mock.GetCacheDb(It.IsAny<string>())).Returns(_cacheDb.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(_computeHistory.Object);

        // Act
        new ComputeHistoryActivity(
            "git",
            new Guid("cbc83480-ae47-46de-91df-60747ca8fb09"),
            analysisLocation.Object
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.GitExecutablePath == "git" &&
                        value.AnalysisLocation != null &&
                        value.AnalysisLocation.CommitId == "75c7fcc7336ee718050c4a5c8dfb5598622787b2"
                )
            )
        );
    }


}
