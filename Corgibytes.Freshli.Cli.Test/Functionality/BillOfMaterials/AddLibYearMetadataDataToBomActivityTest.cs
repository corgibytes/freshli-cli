using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public class AddLibYearMetadataDataToBomActivityTest
{
    private const string PathToBom = "/path/to/bom";
    private const string AgentExecutablePath = "/path/to/agent";

    private readonly CachedManifest _manifest = new() { Id = 12, ManifestFilePath = "/path/to/history-stop-point/path/to/manifest" };

    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly Mock<IApplicationEventEngine> _engine = new();
    private readonly CancellationToken _cancellationToken = new();
    private readonly AddLibYearMetadataDataToBomActivity _activity;

    public AddLibYearMetadataDataToBomActivityTest()
    {
        _activity = new AddLibYearMetadataDataToBomActivity
        {
            Parent = _parent.Object,
            PathToBom = PathToBom,
            AgentExecutablePath = AgentExecutablePath
        };

        _parent.Setup(mock => mock.Manifest).Returns(_manifest);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        var billOfMaterialsProcessor = new Mock<IBillOfMaterialsProcessor>();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IBillOfMaterialsProcessor)))
            .Returns(billOfMaterialsProcessor.Object);
        _engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        await _activity.Handle(_engine.Object, _cancellationToken);

        billOfMaterialsProcessor.Verify(mock =>
            mock.AddLibYearMetadataDataToBom(_manifest, AgentExecutablePath, PathToBom, _cancellationToken)
        );

        _engine.Verify(mock => mock.Fire(
            It.Is<LibYearMetadataAddedToBomEvent>(value =>
                value.Parent == _activity &&
                value.PathToBom == PathToBom &&
                value.AgentExecutablePath == AgentExecutablePath
            ),
            _cancellationToken,
            ApplicationTaskMode.Tracked
        ));
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleFiresHistoryStopPointProcessingFailedOnException()
    {
        var exception = new Exception("Sample exception");
        _engine.Setup(mock => mock.ServiceProvider).Throws(exception);

        await _activity.Handle(_engine.Object, _cancellationToken);

        _engine.Verify(mock =>
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
