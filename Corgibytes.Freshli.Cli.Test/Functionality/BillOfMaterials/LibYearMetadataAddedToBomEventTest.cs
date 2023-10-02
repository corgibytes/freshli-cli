using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

[UnitTest]
public class LibYearMetadataAddedToBomEventTest
{
    private readonly CancellationToken _cancellationToken = new();
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly Mock<IApplicationActivityEngine> _activityClient = new();
    private const string AgentExecutablePath = "/path/to/agent";
    private const string PathToBom = "/path/to/bom";

    [Fact]
    public async Task Handle()
    {
        var appEvent = new LibYearMetadataAddedToBomEvent
        {
            Parent = _parent.Object,
            AgentExecutablePath = AgentExecutablePath,
            PathToBom = PathToBom
        };

        await appEvent.Handle(_activityClient.Object, _cancellationToken);

        _activityClient.Verify(mock =>
            mock.Dispatch(
                It.Is<UploadBomToApiActivity>(value =>
                    value.Parent == appEvent &&
                    value.AgentExecutablePath == AgentExecutablePath &&
                    value.PathToBom == PathToBom
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var exception = new Exception("Sample exception");

        _activityClient.Setup(mock =>
            mock.Dispatch(It.IsAny<UploadBomToApiActivity>(), _cancellationToken, ApplicationTaskMode.Tracked)
        ).Throws(exception);

        var appEvent = new LibYearMetadataAddedToBomEvent
        {
            Parent = _parent.Object,
            AgentExecutablePath = AgentExecutablePath,
            PathToBom = PathToBom
        };

        await appEvent.Handle(_activityClient.Object, _cancellationToken);

        _activityClient.Verify(mock =>
            mock.Dispatch(
                It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
                    value.Parent == appEvent &&
                    value.Error == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
