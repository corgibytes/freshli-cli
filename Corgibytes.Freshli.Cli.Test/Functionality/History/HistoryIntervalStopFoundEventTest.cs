using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class HistoryIntervalStopFoundEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleFiresCreateApiHistoryIntervalStop()
    {
        var historyStopPoint = new CachedHistoryStopPoint { Id = 29 };
        var appEvent = new HistoryIntervalStopFoundEvent
        {
            HistoryStopPoint = historyStopPoint
        };

        var eventClient = new Mock<IApplicationActivityEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<HistoryIntervalStopFoundEvent>>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<HistoryIntervalStopFoundEvent>)))
            .Returns(logger.Object);
        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var cancellationToken = new System.Threading.CancellationToken(false);
        await appEvent.Handle(eventClient.Object, cancellationToken);
    }
}
