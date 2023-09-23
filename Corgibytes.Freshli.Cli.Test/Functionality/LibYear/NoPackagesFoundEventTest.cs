using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

public class NoPackagesFoundEventTest
{
    private readonly Mock<IApplicationActivityEngine> _activityEngine = new();
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly NoPackagesFoundEvent _appEvent;
    private readonly CancellationToken _cancellationToken = new();

    public NoPackagesFoundEventTest()
    {
        _appEvent = new NoPackagesFoundEvent(_parent.Object);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        await _appEvent.Handle(_activityEngine.Object, _cancellationToken);

        _activityEngine.VerifyNoOtherCalls();
    }
}
