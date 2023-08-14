using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AnalysisStartedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleDispatchesCreateAnalysisApiActivity()
    {
        var cancellationToken = new System.Threading.CancellationToken(false);
        var eventClient = new Mock<IApplicationActivityEngine>();

        var analysisStartedEvent = new AnalysisStartedEvent { AnalysisId = Guid.NewGuid() };
        await analysisStartedEvent.Handle(eventClient.Object, cancellationToken);

        eventClient.Verify(
            mock => mock.Dispatch(
                It.Is<CreateAnalysisApiActivity>(value =>
                    value.CachedAnalysisId == analysisStartedEvent.AnalysisId
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
