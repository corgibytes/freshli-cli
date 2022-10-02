using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
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
        var historyStopData = new Mock<IHistoryStopData>();
        var appEvent = new ApiHistoryStopCreatedEvent(cachedAnalysisId, historyStopData.Object);

        var eventClient = new Mock<IApplicationActivityEngine>();

        appEvent.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Dispatch(
            It.Is<CheckoutHistoryActivity>(
                value => value.HistoryStopData == historyStopData.Object)));

    }
}
