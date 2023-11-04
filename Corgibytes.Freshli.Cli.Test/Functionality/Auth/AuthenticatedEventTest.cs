using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Auth;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Auth;

[UnitTest]
public class AuthenticatedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        var authenticatedEvent = new AuthenticatedEvent
        {
            AnalysisId = new Guid(),
            RepositoryUrl = "https://github.com/corgibytes/freshli-fixture-java-test",
            Person = new Person()
        };

        var cancellationToken = new CancellationToken(false);

        var engine = new Mock<IApplicationActivityEngine>();
        await authenticatedEvent.Handle(engine.Object, cancellationToken);

        engine.Verify(mock =>
            mock.Dispatch(
                It.Is<DetermineProjectActivity>(value =>
                    value.AnalysisId == authenticatedEvent.AnalysisId &&
                    value.RepositoryUrl == authenticatedEvent.RepositoryUrl &&
                    value.Person == authenticatedEvent.Person
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
