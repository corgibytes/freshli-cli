using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class CacheWasNotPreparedEventTest
{
    [Fact]
    public void CorrectlyDispatchesRestartAnalysisActivity()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var cacheManager = new Mock<ICacheManager>();
        var historyIntervalParser = new Mock<IHistoryIntervalParser>();

        var cacheEvent = new CacheWasNotPreparedEvent
        {
            CacheDirectory = "example",
            RepositoryUrl = "https://git.example.com",
            RepositoryBranch = "main",
            HistoryInterval = "1m"
        };

        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IHistoryIntervalParser)))
            .Returns(historyIntervalParser.Object);

        var engine = new Mock<IApplicationActivityEngine>();
        engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        cacheEvent.Handle(engine.Object);

        // TODO: Verify that it dispatches PrepareCacheActivity after it's been implemented

        engine.Verify(mock => mock.Dispatch(It.Is<RestartAnalysisActivity>(value =>
            value.CacheManager == cacheManager.Object &&
            value.HistoryIntervalParser == historyIntervalParser.Object &&
            value.CacheDirectory == "example" &&
            value.RepositoryUrl == "https://git.example.com" &&
            value.RepositoryBranch == "main" &&
            value.HistoryInterval == "1m")));
    }
}
