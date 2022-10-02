using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class CreateApiHistoryStopActivtyTest
{
    [Fact]
    public void Handle()
    {
        var configuration = new Mock<IConfiguration>();

        var cachedAnalysisId = Guid.NewGuid();
        var apiAnalysisId = Guid.NewGuid();

        var repositoryUrl = "repository-url";
        var repositoryBranch = "branch";
        var repositoryId = "repository-id";
        var commitId = "commit-id";
        var moment = new DateTimeOffset(2022, 1, 1, 12, 52, 28, 0, TimeSpan.Zero);

        var historyStopData = new HistoryStopData(configuration.Object, repositoryId, commitId, moment);

        var cachedAnalysis = new CachedAnalysis(repositoryUrl, repositoryBranch, "1m", CommitHistory.AtInterval, RevisionHistoryMode.AllRevisions)
        {
            ApiAnalysisId = apiAnalysisId
        };

        var cacheDb = new Mock<ICacheDb>();
        cacheDb.Setup(mock => mock.RetrieveAnalysis(cachedAnalysisId)).Returns(cachedAnalysis);

        var cacheManager = new Mock<ICacheManager>();
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);

        var resultsApi = new Mock<IResultsApi>();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(resultsApi.Object);

        var eventClient = new Mock<IApplicationEventEngine>();
        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var activity = new CreateApiHistoryStopActivity(cachedAnalysisId, historyStopData);

        activity.Handle(eventClient.Object);

        resultsApi.Verify(mock => mock.CreateHistoryPoint(apiAnalysisId, moment));

        eventClient.Verify(mock =>
            mock.Fire(It.Is<ApiHistoryStopCreatedEvent>(value =>
                value.HistoryStopData == historyStopData)));

    }
}
