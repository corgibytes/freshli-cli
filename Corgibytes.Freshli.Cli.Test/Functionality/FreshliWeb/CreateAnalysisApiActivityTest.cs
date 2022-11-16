using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class CreateAnalysisApiActivityTest
{
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    public CreateAnalysisApiActivityTest() =>
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);

    [Fact]
    public void HandleSendsRequest()
    {
        const string url = "anything";
        const string branch = "anythingelse";
        var api = new Mock<IResultsApi>();
        var apiAnalysisId = Guid.NewGuid();
        var cachedAnalysisId = Guid.NewGuid();
        api.Setup(mock => mock.CreateAnalysis(url)).Returns(apiAnalysisId);
        var activity = new CreateAnalysisApiActivity(cachedAnalysisId);

        var cachedAnalysis =
            new CachedAnalysis(url, branch, null!, CommitHistory.AtInterval, RevisionHistoryMode.OnlyLatestRevision)
            {
                Id = cachedAnalysisId
            };
        var cacheDb = new Mock<ICacheDb>();
        cacheDb.Setup(mock => mock.RetrieveAnalysis(cachedAnalysisId)).Returns(cachedAnalysis);

        var cacheManager = new Mock<ICacheManager>();
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);

        _serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(api.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        activity.Handle(_eventEngine.Object);

        cacheDb.Verify(mock => mock.SaveAnalysis(It.Is<CachedAnalysis>(value =>
            value.Id == cachedAnalysisId &&
            value.ApiAnalysisId == apiAnalysisId &&
            value.RepositoryUrl == url &&
            value.RepositoryBranch == branch
        )));

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<AnalysisApiCreatedEvent>(value =>
                value.AnalysisId == cachedAnalysisId
            )));
    }
}
