using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class ProjectDeterminedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CorrectlyDispatchesCloneGitRepositoryActivity()
    {
        var appEvent = new ProjectDeterminedEvent
        {
            AnalysisId = new Guid(),
            RepositoryUrl = "https://github.com/corgibytes/freshli-fixture-java-test",
            ProjectSlug = "test-org/test-project"
        };

        var cancellationToken = new CancellationToken(false);
        var engine = new Mock<IApplicationActivityEngine>();
        await appEvent.Handle(engine.Object, cancellationToken);
        engine.Verify(mock =>
            mock.Dispatch(
                It.Is<CloneGitRepositoryActivity>(value =>
                    value.AnalysisId == appEvent.AnalysisId &&
                    value.ProjectSlug == appEvent.ProjectSlug
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
        var appEvent = new ProjectDeterminedEvent
        {
            AnalysisId = new Guid(),
            RepositoryUrl = temporaryLocation.FullName,
            ProjectSlug = "test-org/test-project"
        };
        var cancellationToken = new CancellationToken(false);
        var engine = new Mock<IApplicationActivityEngine>();
        await appEvent.Handle(engine.Object, cancellationToken);
        engine.Verify(mock =>
            mock.Dispatch(
                It.Is<VerifyGitRepositoryInLocalDirectoryActivity>(value =>
                    value.AnalysisId == appEvent.AnalysisId &&
                    value.ProjectSlug == appEvent.ProjectSlug
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
        temporaryLocation.Delete();
    }
}
