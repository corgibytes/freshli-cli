using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
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
        var historyStopPoint = new CachedHistoryStopPoint { Id = 29 };
        var appEvent = new ApiHistoryStopCreatedEvent
        {
            CachedAnalysisId = cachedAnalysisId,
            HistoryStopPoint = historyStopPoint
        };

        var eventClient = new Mock<IApplicationActivityEngine>();

        var cancellationToken = new System.Threading.CancellationToken(false);

        await appEvent.Handle(eventClient.Object, cancellationToken);

        eventClient.Verify(mock =>
            mock.Dispatch(
                It.Is<CheckoutHistoryActivity>(value =>
                    value.HistoryStopPoint == historyStopPoint
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
