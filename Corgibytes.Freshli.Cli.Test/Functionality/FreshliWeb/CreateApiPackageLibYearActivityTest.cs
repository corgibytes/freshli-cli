using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class CreateApiPackageLibYearActivityTest
{
    private readonly Guid _analysisId = Guid.NewGuid();
    private readonly CachedPackageLibYear _packageLibYear = new() { Id = 9 };
    private const string AgentExecutablePath = "/path/to/agent";
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly Mock<IApplicationEventEngine> _eventClient = new();
    private readonly CancellationToken _cancellationToken = new(false);

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyCallsApiAndFiresApiPackageLibYearCreatedEvent()
    {
        var apiAnalysisId = Guid.NewGuid();

        const string repositoryUrl = "https://url/for/repository";
        const string repositoryBranch = "main";
        const string historyInterval = "1m";

        var cachedAnalysis = new CachedAnalysis
        {
            Id = _analysisId,
            RepositoryUrl = repositoryUrl,
            RepositoryBranch = repositoryBranch,
            HistoryInterval = historyInterval,
            UseCommitHistory = CommitHistory.AtInterval,
            RevisionHistoryMode = RevisionHistoryMode.AllRevisions,
            ApiAnalysisId = apiAnalysisId
        };

        var activity = new CreateApiPackageLibYearActivity
        {
            Parent = _parent.Object,
            PackageLibYear = _packageLibYear,
            AgentExecutablePath = AgentExecutablePath
        };

        var serviceProvider = new Mock<IServiceProvider>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var resultsApi = new Mock<IResultsApi>();

        var historyStopPoint = new CachedHistoryStopPoint { Id = 29, CachedAnalysis = cachedAnalysis };
        _parent.Setup(mock => mock.HistoryStopPoint).Returns(historyStopPoint);

        cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(cacheDb.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(resultsApi.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        await activity.Handle(_eventClient.Object, _cancellationToken);

        resultsApi.Verify(mock =>
            mock.CreatePackageLibYear(cacheDb.Object, _analysisId, historyStopPoint, _packageLibYear)
        );

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<ApiPackageLibYearCreatedEvent>(value =>
                    value.Parent == activity &&
                    value.PackageLibYear == _packageLibYear &&
                    value.AgentExecutablePath == AgentExecutablePath
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleFiresHistoryStopPointProcessingFailedEvent()
    {
        var activity = new CreateApiPackageLibYearActivity
        {
            Parent = _parent.Object,
            PackageLibYear = _packageLibYear,
            AgentExecutablePath = AgentExecutablePath
        };

        var exception = new InvalidOperationException();
        _eventClient.Setup(mock => mock.ServiceProvider).Throws(exception);

        await activity.Handle(_eventClient.Object, _cancellationToken);

        _eventClient.Verify(mock =>
            mock.Fire(
                It.Is<HistoryStopPointProcessingFailedEvent>(value =>
                    value.Parent == activity &&
                    value.Exception == exception
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
