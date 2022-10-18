using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class PrepareCacheForAnalysisActivityTest
{
    [Fact]
    public void VerifyItFiresCachePreparedEvent()
    {
        var eventClient = new Mock<IApplicationEventEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        var configuration = new Mock<IConfiguration>();
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();

        eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        configuration.Setup(mock => mock.CacheDir).Returns("example");
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(configuration.Object);

        var historyInterval = "2y";
        var repositoryBranch = "trunk";
        var repositoryUrl = "https://repository.com";

        var activity = new PrepareCacheForAnalysisActivity
        {
            HistoryInterval = historyInterval,
            RepositoryBranch = repositoryBranch,
            RepositoryUrl = repositoryUrl,
            RevisionHistoryMode = RevisionHistoryMode.OnlyLatestRevision,
            UseCommitHistory = CommitHistory.AtInterval
        };

        activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<CachePreparedForAnalysisEvent>(value =>
            value.HistoryInterval == historyInterval &&
            value.RepositoryBranch == repositoryBranch &&
            value.RepositoryUrl == repositoryUrl &&
            value.RevisionHistoryMode == RevisionHistoryMode.OnlyLatestRevision &&
            value.UseCommitHistory == CommitHistory.AtInterval
        )));
    }
}
