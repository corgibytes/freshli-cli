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
    private readonly PackageURL _package = new("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
    private readonly ComputeLibYearForPackageActivity _activity;
    private readonly Mock<IApplicationEventEngine> _eventClient = new();
    private static readonly DateTimeOffset s_asOfDateTime = new(2021, 1, 29, 12, 30, 45, 0, TimeSpan.Zero);
    private readonly CachedHistoryStopPoint _historyStopPoint = new() { Id = 29, AsOfDateTime = s_asOfDateTime };
    private readonly CachedManifest _manifest = new() { Id = 12, ManifestFilePath = "path/to/manifest" };

    public ComputeLibYearForPackageActivityTest()
    {
        _activity = new ComputeLibYearForPackageActivity
        {
            Parent = _parent.Object,
            AgentExecutablePath = AgentExecutablePath,
            Package = _package
        };

        _parent.Setup(mock => mock.HistoryStopPoint).Returns(_historyStopPoint);
        _parent.Setup(mock => mock.Manifest).Returns(_manifest);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleComputesLibYearAndFiresLibYearComputedForPackageEvent()
    {
        var packageLibYear = new PackageLibYear(
            s_asOfDateTime,
            _package,
            s_asOfDateTime,
            _package,
            6.2,
            s_asOfDateTime
        );

        var serviceProvider = new Mock<IServiceProvider>();
        var calculator = new Mock<IPackageLibYearCalculator>();
        var agentManager = new Mock<IAgentManager>();
        var agentReader = new Mock<IAgentReader>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();

        agentManager.Setup(mock => mock.GetReader(AgentExecutablePath, CancellationToken.None)).Returns(agentReader.Object);
        agentManager.Setup(mock => mock.GetLibYearCalculator(agentReader.Object, _package, s_asOfDateTime)).Returns(calculator.Object);

        calculator.Setup(mock => mock.ComputeLibYear()).ReturnsAsync(packageLibYear);
        cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrievePackageLibYear(_package, s_asOfDateTime)).ReturnsAsync((CachedPackageLibYear?)null);
        var cachedPackageLibYear = new CachedPackageLibYear()
        {
            Id = 22,
            PackageUrl = _package.ToString()!,
            AsOfDateTime = s_asOfDateTime,
            ReleaseDateCurrentVersion = packageLibYear.ReleaseDateCurrentVersion,
            LatestVersion = packageLibYear.LatestVersion.ToString(),
            ReleaseDateLatestVersion = packageLibYear.ReleaseDateLatestVersion,
            LibYear = packageLibYear.LibYear
        };
        cacheDb.Setup(mock => mock.AddPackageLibYear(_manifest, It.IsAny<CachedPackageLibYear>())).ReturnsAsync(
            cachedPackageLibYear);

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IPackageLibYearCalculator))).Returns(calculator.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        cacheDb.Verify(mock => mock.AddPackageLibYear(
            It.Is<CachedManifest>(value => value.Id == _manifest.Id),
            It.Is<CachedPackageLibYear>(value =>
                value.PackageUrl == _package.ToString() &&
                value.AsOfDateTime == s_asOfDateTime &&
                value.ReleaseDateCurrentVersion == packageLibYear.ReleaseDateCurrentVersion &&
                value.LatestVersion == packageLibYear.LatestVersion.ToString() &&
                value.ReleaseDateLatestVersion == packageLibYear.ReleaseDateLatestVersion &&
                Math.Abs(value.LibYear - packageLibYear.LibYear) < 0.1)));

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<LibYearComputedForPackageEvent>(value =>
                    value.Parent == _activity &&
                    value.PackageLibYear == cachedPackageLibYear &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleComputesUsesLibYearFromCacheAndFiresLibYearComputedForPackageEvent()
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

        var serviceProvider = new Mock<IServiceProvider>();
        var calculator = new Mock<IPackageLibYearCalculator>();
        var agentManager = new Mock<IAgentManager>();
        var agentReader = new Mock<IAgentReader>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint { Id = 29, AsOfDateTime = asOfDateTime };

        agentManager.Setup(mock => mock.GetReader(AgentExecutablePath, CancellationToken.None)).Returns(agentReader.Object);
        agentManager.Setup(mock => mock.GetLibYearCalculator(agentReader.Object, _package, asOfDateTime)).Returns(calculator.Object);

        calculator.Setup(mock => mock.ComputeLibYear()).ReturnsAsync(packageLibYear);
        cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).ReturnsAsync(historyStopPoint);
        var cachedPackageLibYear = new CachedPackageLibYear
        {
            PackageUrl = _package.ToString()!,
            AsOfDateTime = asOfDateTime,
            ReleaseDateCurrentVersion = packageLibYear.ReleaseDateCurrentVersion,
            LatestVersion = packageLibYear.LatestVersion.ToString(),
            ReleaseDateLatestVersion = packageLibYear.ReleaseDateLatestVersion,
            LibYear = packageLibYear.LibYear
        };
        cacheDb.Setup(mock => mock.RetrievePackageLibYear(_package, asOfDateTime)).ReturnsAsync(cachedPackageLibYear);

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IPackageLibYearCalculator))).Returns(calculator.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IAgentManager))).Returns(agentManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        await _activity.Handle(_eventClient.Object, _cancellationToken);

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<LibYearComputedForPackageEvent>(value =>
                    value.Parent == _activity &&
                    value.PackageLibYear == cachedPackageLibYear &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleFiresProcessingErrorEventOnException()
    {
        var exception = new InvalidOperationException();
        _eventClient.Setup(mock => mock.ServiceProvider).Throws(exception);

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
