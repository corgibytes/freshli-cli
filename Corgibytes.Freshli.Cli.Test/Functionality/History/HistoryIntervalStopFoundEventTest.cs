using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class HistoryIntervalStopFoundEventTest
{
    [Fact]
    public async Task HandleFiresCreateApiHistoryIntervalStop()
    {
        var cachedAnalysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var appEvent = new HistoryIntervalStopFoundEvent(cachedAnalysisId, historyStopPointId);

        var eventClient = new Mock<IApplicationActivityEngine>();

        await appEvent.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Dispatch(
            It.Is<CreateApiHistoryStopActivity>(value =>
                value.CachedAnalysisId == cachedAnalysisId &&
                value.HistoryStopPointId == historyStopPointId)));
    }
}
