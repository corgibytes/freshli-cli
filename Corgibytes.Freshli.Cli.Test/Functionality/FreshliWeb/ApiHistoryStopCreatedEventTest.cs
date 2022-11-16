using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class ApiHistoryStopCreatedEventTest
{
    [Fact]
    public void HandleDispatchesCheckoutHistoryActivity()
    {
        var cachedAnalysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var appEvent = new ApiHistoryStopCreatedEvent(cachedAnalysisId, historyStopPointId);

        var eventClient = new Mock<IApplicationActivityEngine>();

        appEvent.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Dispatch(
            It.Is<CheckoutHistoryActivity>(
                value => value.HistoryStopPointId == historyStopPointId)));
    }
}
