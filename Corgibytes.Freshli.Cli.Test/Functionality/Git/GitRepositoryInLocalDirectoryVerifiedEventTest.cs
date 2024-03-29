using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class GitRepositoryInLocalDirectoryVerifiedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyItFiresComputeHistoryActivity()
    {
        var analysisId = Guid.NewGuid();
        var localDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        var verifyEvent = new GitRepositoryInLocalDirectoryVerifiedEvent();

        var engine = new Mock<IApplicationActivityEngine>();
        var cancellationToken = new System.Threading.CancellationToken(false);
        await verifyEvent.Handle(engine.Object, cancellationToken);

        engine.Setup(mock =>
            mock.Dispatch(
                It.Is<ComputeHistoryActivity>(value =>
                    value.AnalysisId == analysisId &&
                    value.HistoryStopData.Path == localDirectory
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
