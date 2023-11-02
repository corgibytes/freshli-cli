using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AnalysisStartedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CorrectlyDispatchesCloneGitRepositoryActivity()
    {
        var startedEvent = new AnalysisStartedEvent
        {
            AnalysisId = new Guid(),
            RepositoryUrl = "https://github.com/corgibytes/freshli-fixture-java-test"
        };

        var cancellationToken = new System.Threading.CancellationToken(false);

        var engine = new Mock<IApplicationActivityEngine>();
        await startedEvent.Handle(engine.Object, cancellationToken);

        engine.Verify(mock =>
            mock.Dispatch(
                It.Is<CloneGitRepositoryActivity>(value =>
                    value.CachedAnalysisId == startedEvent.AnalysisId
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CorrectlyDispatchesVerifyGitRepositoryInLocalDirectoryActivity()
    {
        var temporaryLocation = new DirectoryInfo(Path.Combine(Path.GetTempPath(), new Guid().ToString()));
        temporaryLocation.Create();

        var startedEvent = new AnalysisStartedEvent
        {
            AnalysisId = new Guid(),
            RepositoryUrl = temporaryLocation.FullName
        };

        var cancellationToken = new System.Threading.CancellationToken(false);

        var engine = new Mock<IApplicationActivityEngine>();
        await startedEvent.Handle(engine.Object, cancellationToken);

        engine.Verify(mock =>
            mock.Dispatch(
                It.Is<VerifyGitRepositoryInLocalDirectoryActivity>(value =>
                    value.AnalysisId == startedEvent.AnalysisId
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        temporaryLocation.Delete();
    }
}
