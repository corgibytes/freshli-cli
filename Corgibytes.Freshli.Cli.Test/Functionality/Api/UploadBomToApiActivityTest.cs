using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Api;

[UnitTest]
public class UploadBomToApiActivityTest
{
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly CachedManifest _manifest = new();
    private readonly CancellationToken _cancellationToken = new();
    private readonly Mock<IApplicationEventEngine> _eventClient = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly UploadBomToApiActivity _activity;
    private const string AgentExecutablePath = "/path/to/agent";
    private const string PathToBom = "/path/to/bom";

    public UploadBomToApiActivityTest()
    {
        _parent.Setup(mock => mock.Manifest).Returns(_manifest);
        _eventClient.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _activity = new UploadBomToApiActivity
        {
            Parent = _parent.Object,
            AgentExecutablePath = AgentExecutablePath,
            PathToBom = PathToBom
        };
    }

    [Fact]
    public async Task Handle()
    {
        var resultsApi = new Mock<IResultsApi>();

        _serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(resultsApi.Object);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        resultsApi.Verify(mock => mock.UploadBomForManifest(_manifest, PathToBom));

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<BomUploadedToApiEvent>(value => value.Parent == _activity),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var exception = new Exception("sample exception");

        _serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Throws(exception);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(value =>
                    value.Parent == _activity &&
                    value.Exception == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
