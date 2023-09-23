using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class BillOfMaterialsGeneratedEventTest
{
    private const string PathToBom = "/path/to/bom";
    private const string AgentExecutablePath = "/path/to/agent";
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly CancellationToken _cancellationToken = new(false);
    private readonly Mock<IApplicationActivityEngine> _engine = new();
    private readonly BillOfMaterialsGeneratedEvent _appEvent;

    public BillOfMaterialsGeneratedEventTest()
    {
        _appEvent = new BillOfMaterialsGeneratedEvent
        {
            Parent = _parent.Object,
            PathToBillOfMaterials = PathToBom,
            AgentExecutablePath = AgentExecutablePath
        };
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CorrectlyDispatchesComputeLibYearActivity()
    {
        await _appEvent.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock =>
            mock.Dispatch(
                It.Is<DeterminePackagesFromBomActivity>(value =>
                    value.Parent == _appEvent &&
                    value.PathToBom == PathToBom &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var exception = new InvalidOperationException();
        _engine.Setup(
            mock => mock.Dispatch(
                It.IsAny<DeterminePackagesFromBomActivity>(),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        ).Throws(exception);

        await _appEvent.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock =>
            mock.Dispatch(
                It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
                    value.Parent == _appEvent &&
                    value.Error == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
