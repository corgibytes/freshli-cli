using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

public class ComputeHistoryActivityTest
{
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IComputeHistory> _computeHistory = new();
    private readonly Mock<ICacheDb> _cacheDb = new();

    [Fact]
    public void FiresHistoryIntervalStopFoundEvents()
    {
        const string repositoryId = "2dbc2fd2358e1ea1b7a6bc08ea647b9a337ac92d";

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
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(historyIntervalStops);

        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value => value.GitCommitIdentifier == "75c7fcc7336ee718050c4a5c8dfb5598622787b2" && value.RepositoryId == repositoryId
                    )
                )
            );
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<HistoryIntervalStopFoundEvent>(
                    value => value.GitCommitIdentifier == "583d813db3e28b9b44a29db352e2f0e1b4c6e420" && value.RepositoryId == repositoryId
                )
            )
        );
    }
}

