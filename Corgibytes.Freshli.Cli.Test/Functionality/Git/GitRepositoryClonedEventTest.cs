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
        var gitPath = "test";
        var cacheDir = "example";
        var gitRepositoryId = "example";
        var analysisId = Guid.NewGuid();
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(mock => mock.GitPath).Returns(gitPath);
        configuration.Setup(mock => mock.CacheDir).Returns(cacheDir);
        var historyStopData = new HistoryStopData(configuration.Object, gitRepositoryId);

        var clonedEvent = new GitRepositoryClonedEvent
        {
            AnalysisId = analysisId,
            HistoryStopData = historyStopData
        };

        var engine = new Mock<IApplicationActivityEngine>();

        clonedEvent.Handle(engine.Object);

        // Verify that it dispatches ComputeHistoryActivity
        engine.Verify(mock => mock.Dispatch(It.Is<ComputeHistoryActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopData == historyStopData)));
    }
}
