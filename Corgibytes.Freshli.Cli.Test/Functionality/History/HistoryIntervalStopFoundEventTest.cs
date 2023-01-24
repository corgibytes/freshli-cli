using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class HistoryIntervalStopFoundEventTest
{
    [Fact(Timeout = 500)]
    public async Task HandleFiresCreateApiHistoryIntervalStop()
    {
        var cachedAnalysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var appEvent = new HistoryIntervalStopFoundEvent(cachedAnalysisId, historyStopPointId);

        var eventClient = new Mock<IApplicationActivityEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<HistoryIntervalStopFoundEvent>>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<HistoryIntervalStopFoundEvent>)))
            .Returns(logger.Object);
        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        await appEvent.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Dispatch(
            It.Is<CreateApiHistoryStopActivity>(value =>
                value.CachedAnalysisId == cachedAnalysisId &&
                value.HistoryStopPointId == historyStopPointId)));
    }
}
