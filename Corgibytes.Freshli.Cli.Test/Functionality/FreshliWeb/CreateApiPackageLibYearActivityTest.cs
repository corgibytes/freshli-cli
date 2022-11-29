using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class CreateApiPackageLibYearActivityTest
{
    [Fact]
    public async Task HandleCorrectlyCallsApiAndFiresApiPackageLibYearCreatedEvent()
    {
        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 12;
        const int packageLibYearId = 9;
        const string agentExecutablePath = "/path/to/agent";
        var apiAnalysisId = Guid.NewGuid();

        const string repositoryUrl = "https://url/for/repository";
        const string repositoryBranch = "main";
        const string historyInterval = "1m";

        var cachedAnalysis = new CachedAnalysis(repositoryUrl, repositoryBranch, historyInterval,
            CommitHistory.AtInterval, RevisionHistoryMode.AllRevisions)
        { ApiAnalysisId = apiAnalysisId };

        var activity = new CreateApiPackageLibYearActivity
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            PackageLibYearId = packageLibYearId,
            AgentExecutablePath = agentExecutablePath
        };

        var serviceProvider = new Mock<IServiceProvider>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var resultsApi = new Mock<IResultsApi>();
        var eventClient = new Mock<IApplicationEventEngine>();

        cacheDb.Setup(mock => mock.RetrieveAnalysis(analysisId)).ReturnsAsync(cachedAnalysis);
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        resultsApi.Setup(mock => mock.CreatePackageLibYear(cacheDb.Object, analysisId, packageLibYearId));
        serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(resultsApi.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        await activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<ApiPackageLibYearCreatedEvent>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.PackageLibYearId == packageLibYearId &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }
}
