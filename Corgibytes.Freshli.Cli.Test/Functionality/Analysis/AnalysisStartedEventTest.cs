using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Auth;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AnalysisStartedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
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
                It.Is<EnsureAuthenticatedActivity>(value =>
                    value.AnalysisId == startedEvent.AnalysisId &&
                    value.RepositoryUrl == startedEvent.RepositoryUrl
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
