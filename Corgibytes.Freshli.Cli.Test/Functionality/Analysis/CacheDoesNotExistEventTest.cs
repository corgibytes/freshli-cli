using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class CacheWasNotPreparedEventTest
{
    [Fact(Timeout = 500)]
    public async Task CorrectlyDispatchesPrepareCacheActivity()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var cacheManager = new Mock<ICacheManager>();
        var historyIntervalParser = new Mock<IHistoryIntervalParser>();

        var cacheEvent = new CacheDoesNotExistEvent
        {
            RepositoryUrl = "https://git.example.com",
            RepositoryBranch = "main",
            HistoryInterval = "1m",
            UseCommitHistory = CommitHistory.Full
        };

        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IHistoryIntervalParser)))
            .Returns(historyIntervalParser.Object);

        var engine = new Mock<IApplicationActivityEngine>();
        engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        await cacheEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<PrepareCacheForAnalysisActivity>(value =>
            value.RepositoryUrl == cacheEvent.RepositoryUrl &&
            value.RepositoryBranch == cacheEvent.RepositoryBranch &&
            value.HistoryInterval == cacheEvent.HistoryInterval &&
            value.UseCommitHistory == cacheEvent.UseCommitHistory)));
    }
}
