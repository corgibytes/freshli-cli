using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class ErrorEventTest
{
    [Fact(Timeout = 500)]
    public async Task CorrectlyDispatchesLogAnalysisFailureActivity()
    {
        var cancellationToken = new System.Threading.CancellationToken(false);
        var errorEvent = new InvalidHistoryIntervalEvent { ErrorMessage = "Uh-oh!" };
        var engine = new Mock<IApplicationActivityEngine>();

        await errorEvent.Handle(engine.Object, cancellationToken);

        engine.Verify(
            mock => mock.Dispatch(
                It.Is<LogAnalysisFailureActivity>(
                    value => value.ErrorEvent.ErrorMessage == errorEvent.ErrorMessage
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
