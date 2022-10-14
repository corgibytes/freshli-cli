using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class CreateApiPackageLibYearActivityTest
{
    [Fact]
    public void HandleCorrectlyCallsApiAndFiresApiPackageLibYearCreatedEvent()
    {
        var analysisId = Guid.NewGuid();
        var historyStopPointId = 12;
        var agentExecutablePath = "/path/to/agent";
        var apiAnalysisId = Guid.NewGuid();

        // note: these dates are completely fabricated
        var releaseDateCurrentVersion = new DateTimeOffset(2021, 12, 23, 11, 22, 33, 44, TimeSpan.Zero);
        var currentVersion =
            new PackageURL("pkg:maven/org.apache.xmlgraphics/batik-anim@1.9.1?repository_url=repo.spring.io%2Frelease");
        var releaseDateLatestVersion = new DateTimeOffset(2021, 12, 24, 11, 22, 33, 44, TimeSpan.Zero);
        var latestVersion =
            new PackageURL("pkg:maven/org.apache.xmlgraphics/batik-anim@1.10?repository_url=repo.spring.io%2Frelease");
        var libYear = 1.2;
        var asOfDateTime = new DateTimeOffset(2021, 12, 25, 11, 22, 33, 44, TimeSpan.Zero);

        var packageLibYear = new PackageLibYear(releaseDateCurrentVersion, currentVersion, releaseDateLatestVersion,
            latestVersion, libYear, asOfDateTime);

        var repositoryUrl = "https://url/for/repository";
        var repositoryBranch = "main";
        var historyInterval = "1m";

        var cachedAnalysis = new CachedAnalysis(repositoryUrl, repositoryBranch, historyInterval,
            CommitHistory.AtInterval, RevisionHistoryMode.AllRevisions)
        {
            ApiAnalysisId = apiAnalysisId
        };

        var activity = new CreateApiPackageLibYearActivity()
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            AgentExecutablePath = agentExecutablePath,
            PackageLibYear = packageLibYear
        };

        var serviceProvider = new Mock<IServiceProvider>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var resultsApi = new Mock<IResultsApi>();
        var eventClient = new Mock<IApplicationEventEngine>();

        cacheDb.Setup(mock => mock.RetrieveAnalysis(analysisId)).Returns(cachedAnalysis);
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        resultsApi.Setup(mock => mock.CreatePackageLibYear(apiAnalysisId, asOfDateTime, packageLibYear));
        serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(resultsApi.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<ApiPackageLibYearCreatedEvent>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.AgentExecutablePath == agentExecutablePath &&
            value.PackageLibYear == packageLibYear
        )));
    }
}
