using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AnalysisStartedEventTest
{
    [Fact]
    public void CorrectlyDispatchesCloneGitRepositoryActivity()
    {
        var startedEvent = new AnalysisStartedEvent
        {
            AnalysisId = new Guid(),
            RepositoryUrl = "https://github.com/corgibytes/freshli-fixture-java-test"
        };

        var engine = new Mock<IApplicationActivityEngine>();
        startedEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<CloneGitRepositoryActivity>(value =>
            value.CachedAnalysisId == startedEvent.AnalysisId
        )));
    }

    [Fact]
    public void CorrectlyDispatchesVerifyGitRepositoryInLocalDirectoryActivity()
    {
        var temporaryLocation = new DirectoryInfo(Path.Combine(Path.GetTempPath(), new Guid().ToString()));
        temporaryLocation.Create();

        var startedEvent = new AnalysisStartedEvent
        {
            AnalysisId = new Guid(),
            RepositoryUrl = temporaryLocation.FullName
        };

        var engine = new Mock<IApplicationActivityEngine>();
        startedEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<VerifyGitRepositoryInLocalDirectoryActivity>(value =>
            value.AnalysisId == startedEvent.AnalysisId
        )));

        temporaryLocation.Delete();
    }
}
