using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public class PackagesFromBomProcessedEventTest
{
    private const string PathToBom = "/path/to/bom";
    private const string AgentExecutablePath = "/path/to/agent";

    private readonly Mock<IApplicationActivityEngine> _engine = new();
    private readonly CancellationToken _cancellationToken = new();
    private readonly PackagesFromBomProcessedEvent _applicationEvent;

    public PackagesFromBomProcessedEventTest()
    {
        _applicationEvent = new PackagesFromBomProcessedEvent
        {
            Parent = _applicationEvent,
            PathToBom = PathToBom,
            AgentExecutablePath = AgentExecutablePath
        };
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        await _applicationEvent.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock =>
            mock.Dispatch(
                It.Is<AddLibYearMetadataDataToBomActivity>(value =>
                    value.Parent == _applicationEvent &&
                    value.PathToBom == PathToBom &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleFiresHistoryStopPointProcessingFailedOnException()
    {
        var exception = new Exception("Sample exception");
        _engine.Setup(mock =>
            mock.Dispatch(It.IsAny<AddLibYearMetadataDataToBomActivity>(), _cancellationToken, ApplicationTaskMode.Tracked)
        ).Throws(exception);

        await _applicationEvent.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock =>
            mock.Dispatch(
                It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
                    value.Parent == _applicationEvent &&
                    value.Error == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
