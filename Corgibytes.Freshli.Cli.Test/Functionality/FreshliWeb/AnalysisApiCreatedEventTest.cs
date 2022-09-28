using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class AnalysisApiCreatedEventTest
{
    [Fact]
    public void HandleDispatchesGitCloneRepositoryActivity()
    {
        var apiEvent = new AnalysisApiCreatedEvent
        {
            CachedAnalysisId = Guid.NewGuid(),
        };

        var engine = new Mock<IApplicationActivityEngine>();

        apiEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<CloneGitRepositoryActivity>(value =>
            value.AnalysisId == apiEvent.CachedAnalysisId
        )));

    }
}
