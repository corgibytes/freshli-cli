using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[UnitTest]
public class GitRepositoryClonedEventTest
{
    [Fact]
    public void CorrectlyDispatchesComputeHistoryActivity()
    {
        var serviceProvider = new Mock<IServiceProvider>();

        var gitPath = "test";
        var analysisId = new Guid();
        var clonedEvent = new GitRepositoryClonedEvent
        {
            GitRepositoryId = "example",
            AnalysisId = analysisId,
            GitPath = gitPath
        };

        var cacheManager = new Mock<ICacheManager>();
        cacheManager.Setup(mock => mock.GetCacheDb(It.IsAny<string>())).Returns(new CacheDb("example"));
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(cacheManager.Object);

        var computeHistoryService = new Mock<IComputeHistory>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(computeHistoryService.Object);

        var engine = new Mock<IApplicationActivityEngine>();
        engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        clonedEvent.Handle(engine.Object);

        // Verify that it dispatches ComputeHistoryActivity
        engine.Verify(mock => mock.Dispatch(It.Is<ComputeHistoryActivity>(value => true)));
    }
}
