using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class HistoryIntervalStopFoundEventTest
{
    [Fact]
    public void HandleFiresCreateApiHistoryIntervalStop()
    {
        var historyStopData = new Mock<IHistoryStopData>();
        var appEvent = new HistoryIntervalStopFoundEvent(historyStopData.Object);

        var eventClient = new Mock<IApplicationActivityEngine>();

        appEvent.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Dispatch(
            It.Is<CheckoutHistoryActivity>(
                value => value.HistoryStopData == historyStopData.Object)));
    }
}
