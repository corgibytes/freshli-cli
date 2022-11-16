using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class ErrorEventTest
{
    [Fact]
    public async ValueTask CorrectlyDispatchesLogAnalysisFailureActivity()
    {
        var errorEvent = new InvalidHistoryIntervalEvent { ErrorMessage = "Uh-oh!" };
        var engine = new Mock<IApplicationActivityEngine>();

        await errorEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<LogAnalysisFailureActivity>(
            value => value.ErrorEvent.ErrorMessage == errorEvent.ErrorMessage
        )));
    }
}
