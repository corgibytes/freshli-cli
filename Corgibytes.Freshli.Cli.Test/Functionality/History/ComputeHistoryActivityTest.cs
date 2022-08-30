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
        var cachedAnalysis = new CachedAnalysis("https://lorem-ipsum.com", "main", "month");
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
                It.IsAny<IAnalysisLocation>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(historyIntervalStops);

        var analysisLocation = new Mock<IAnalysisLocation>();

        // Act
        new ComputeHistoryActivity(
            "git",
            _cacheDb.Object,
            _computeHistory.Object,
            new Guid("cbc83480-ae47-46de-91df-60747ca8fb09"),
            analysisLocation.Object
        ).Handle(_eventEngine.Object);

        // Assert
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.GitCommitIdentifier == "75c7fcc7336ee718050c4a5c8dfb5598622787b2" && value.AnalysisLocation == analysisLocation.Object
                )
            )
        );
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value =>
                        value.GitCommitIdentifier == "583d813db3e28b9b44a29db352e2f0e1b4c6e420" && value.AnalysisLocation == analysisLocation.Object
                )
            )
        );
    }
}
