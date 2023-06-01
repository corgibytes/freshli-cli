using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class ComputeLibYearForPackageActivityTest
{
    private const string AgentExecutablePath = "/path/to/agent/smith";

    private readonly CancellationToken _cancellationToken = new(false);
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly Guid _analysisId = Guid.NewGuid();
    private readonly PackageURL _package = new("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
    private readonly ComputeLibYearForPackageActivity _activity;
    private readonly Mock<IApplicationEventEngine> _eventClient = new();

    public ComputeLibYearForPackageActivityTest()
    {
        _activity = new ComputeLibYearForPackageActivity
        {
            AnalysisId = _analysisId,
            Parent = _parent.Object,
            AgentExecutablePath = AgentExecutablePath,
            Package = _package
        };
    }

    [Fact(Timeout = 500)]
    public async Task HandleComputesLibYearAndFiresLibYearComputedForPackageEvent()
    {
        var asOfDateTime = new DateTimeOffset(2021, 1, 29, 12, 30, 45, 0, TimeSpan.Zero);
        var packageLibYear = new PackageLibYear(
            asOfDateTime,
            _package,
            asOfDateTime,
            _package,
            6.2,
            asOfDateTime
        );

        const int historyStopPointId = 29;
        const int packageLibYearId = 9;

        var serviceProvider = new Mock<IServiceProvider>();
        var calculator = new Mock<IPackageLibYearCalculator>();
        var agentManager = new Mock<IAgentManager>();
        var agentReader = new Mock<IAgentReader>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { AsOfDateTime = asOfDateTime };

        agentManager.Setup(mock => mock.GetReader(AgentExecutablePath, CancellationToken.None)).Returns(agentReader.Object);
        _parent.Setup(mock => mock.HistoryStopPointId).Returns(historyStopPointId);
        calculator.Setup(mock => mock.ComputeLibYear(agentReader.Object, _package, asOfDateTime))
            .ReturnsAsync(packageLibYear);
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).ReturnsAsync(historyStopPoint);
        cacheDb.Setup(mock => mock.AddPackageLibYear(It.IsAny<CachedPackageLibYear>())).ReturnsAsync(packageLibYearId);

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IPackageLibYearCalculator))).Returns(calculator.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        cacheDb.Verify(mock => mock.AddPackageLibYear(
            It.Is<CachedPackageLibYear>(value =>
                value.PackageName == _package.Name &&
                value.CurrentVersion == packageLibYear.CurrentVersion!.ToString() &&
                value.ReleaseDateCurrentVersion == packageLibYear.ReleaseDateCurrentVersion &&
                value.LatestVersion == packageLibYear.LatestVersion!.ToString() &&
                value.ReleaseDateLatestVersion == packageLibYear.ReleaseDateLatestVersion &&
                Math.Abs(value.LibYear - packageLibYear.LibYear) < 0.1 &&
                value.HistoryStopPointId == historyStopPointId)));

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<LibYearComputedForPackageEvent>(value =>
                    value.AnalysisId == _analysisId &&
                    value.Parent == _parent.Object &&
                    value.PackageLibYearId == packageLibYearId &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = 500)]
    public async Task HandleFiresProcessingErrorEventOnException()
    {
        var exception = new InvalidOperationException();
        _eventClient.Setup(mock => mock.ServiceProvider).Throws(exception);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(value =>
                    value.Parent == _activity.Parent &&
                    value.Exception == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
