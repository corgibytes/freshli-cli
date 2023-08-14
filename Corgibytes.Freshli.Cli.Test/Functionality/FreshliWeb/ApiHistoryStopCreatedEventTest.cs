using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class ApiHistoryStopCreatedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleDispatchesCheckoutHistoryActivity()
    {
        var cachedAnalysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var appEvent = new ApiHistoryStopCreatedEvent(cachedAnalysisId, historyStopPointId);

        var eventClient = new Mock<IApplicationActivityEngine>();

        var cancellationToken = new System.Threading.CancellationToken(false);

        await appEvent.Handle(eventClient.Object, cancellationToken);

        eventClient.Verify(mock =>
            mock.Dispatch(
                It.Is<CheckoutHistoryActivity>(value =>
                    value.HistoryStopPointId == historyStopPointId
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
