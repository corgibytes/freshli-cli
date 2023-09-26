using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class DeterminePackagesFromBomActivityTest
{
    private const string PathToBom = "/path/to/bom";
    private const string PathToAgentExecutable = "/path/to/agent";
    private readonly Mock<IApplicationEventEngine> _eventClient = new();
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly DeterminePackagesFromBomActivity _activity;
    private readonly CancellationToken _cancellationToken = new();
    private readonly CachedHistoryStopPoint _historyStopPoint = new()
    {
        Id = 22,
        CachedAnalysis = new CachedAnalysis { Id = Guid.NewGuid() }
    };

    public DeterminePackagesFromBomActivityTest()
    {
        _activity = new DeterminePackagesFromBomActivity
        {
            Parent = _parent.Object,
            PathToBom = PathToBom,
            AgentExecutablePath = PathToAgentExecutable
        };
        _parent.Setup(mock => mock.HistoryStopPoint).Returns(_historyStopPoint);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyFiresLibYearComputationForBomStartedEvent()
    {
        var serviceProvider = new Mock<IServiceProvider>();

        var packageAlpha = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");

        var packageBeta = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0");

        // mock out a list of packages that are found by the IBomReader

        var bomReader = new Mock<IBomReader>();

        bomReader.Setup(mock => mock.AsPackageUrls(PathToBom)).Returns(new List<PackageURL>
        {
            packageAlpha,
            packageBeta
        });

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IBomReader))).Returns(bomReader.Object);

        _eventClient.Setup(mock => mock.Wait(_activity, _cancellationToken)).Returns(ValueTask.CompletedTask);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<PackageFoundEvent>(value =>
                    value.Parent == _activity &&
                    value.AgentExecutablePath == PathToAgentExecutable &&
                    value.Package == packageAlpha
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<PackageFoundEvent>(value =>
                    value.Parent == _activity &&
                    value.AgentExecutablePath == PathToAgentExecutable &&
                    value.Package == packageBeta
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        Assert.NotNull(_activity.WaitingForChildrenThread);
        _activity.StopWaitingForChildren();

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<PackagesFromBomProcessedEvent>(value =>
                    value.Parent == _activity &&
                    value.PathToBom == _activity.PathToBom &&
                    value.AgentExecutablePath == _activity.AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Untracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleFiresHistoryStopPointProcessingFailedOnException()
    {
        var serviceProvider = new Mock<IServiceProvider>();

        var exception = new InvalidOperationException("Simulated exception");

        var bomReader = new Mock<IBomReader>();
        bomReader.Setup(mock => mock.AsPackageUrls(PathToBom)).Throws(exception);

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IBomReader))).Returns(bomReader.Object);

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

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleWhenNoPackagesAreFound()
    {
        var serviceProvider = new Mock<IServiceProvider>();

        var bomReader = new Mock<IBomReader>();

        bomReader.Setup(mock => mock.AsPackageUrls(PathToBom)).Returns(new List<PackageURL>());

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IBomReader))).Returns(bomReader.Object);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<NoPackagesFoundEvent>(value =>
                    value.Parent == _activity
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
