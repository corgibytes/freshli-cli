using System;
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
        var analysisId = new Guid();
        var clonedEvent = new GitRepositoryClonedEvent
        {
            GitRepositoryId = "example",
            AnalysisId = analysisId,
            GitPath = gitPath,
            CacheDir = cacheDir
        };

        var engine = new Mock<IApplicationActivityEngine>();

        clonedEvent.Handle(engine.Object);

        // Verify that it dispatches ComputeHistoryActivity
        engine.Verify(mock => mock.Dispatch(It.Is<ComputeHistoryActivity>(value =>
            value.AnalysisId == analysisId &&
            value.CacheDir == cacheDir &&
            value.GitExecutablePath == gitPath)));
    }
}
