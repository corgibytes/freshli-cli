using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class CheckoutHistoryActivityTest
{
    [Fact]
    public void Handle()
    {
        var commitId = "abcdef1";
        var gitExecutablePath = "/path/to/git";
        var repositoryId = Guid.NewGuid().ToString();
        var cacheDirectory = "/path/to/cache/dir";
        var archiveLocation = $"{cacheDirectory}/histories/{repositoryId}/{commitId}";

        var configuration = new Mock<IConfiguration>();
        configuration.Setup(mock => mock.GitPath).Returns(gitExecutablePath);
        configuration.Setup(mock => mock.CacheDir).Returns(cacheDirectory);

        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        var historyStopPoint = new CachedHistoryStopPoint
        {
            RepositoryId = repositoryId,
            GitCommitId = commitId
        };

        var historyStopPointId = 29;
        cacheManager.Setup(mock => mock.GetCacheDb()).Returns(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveHistoryStopPoint(historyStopPointId)).Returns(historyStopPoint);

        var gitManager = new Mock<IGitManager>();

        var analysisId = Guid.NewGuid();
        var activity = new CheckoutHistoryActivity(analysisId, historyStopPointId);

        var serviceProvider = new Mock<IServiceProvider>();
        var eventEngine = new Mock<IApplicationEventEngine>();
        eventEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IGitManager))).Returns(gitManager.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var parsedCommitId = new GitCommitIdentifier(commitId);
        gitManager.Setup(mock => mock.ParseCommitId(commitId)).Returns(parsedCommitId);
        gitManager.Setup(
            mock => mock.CreateArchive(repositoryId, parsedCommitId)
        ).Returns(archiveLocation);

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(
            mock => mock.Fire(It.Is<HistoryStopCheckedOutEvent>(appEvent =>
                appEvent.AnalysisId == analysisId &&
                appEvent.HistoryStopPointId == historyStopPointId)));
    }
}
