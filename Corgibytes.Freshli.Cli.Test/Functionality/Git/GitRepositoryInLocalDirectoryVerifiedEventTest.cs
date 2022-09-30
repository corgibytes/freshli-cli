using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class GitRepositoryInLocalDirectoryVerifiedEventTest
{
    [Fact]
    public void VerifyItFiresComputeHistoryActivity()
    {
        var analysisId = Guid.NewGuid();
        var localDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var verifyEvent = new GitRepositoryInLocalDirectoryVerifiedEvent();

        var engine = new Mock<IApplicationActivityEngine>();
        verifyEvent.Handle(engine.Object);

        engine.Setup(mock => mock.Dispatch(It.Is<ComputeHistoryActivity>(value =>
            value.AnalysisId == analysisId &&
            value.AnalysisLocation.Path == localDirectory
        )));
    }
}
