using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class AnalysisStartedEventTest
{
    [Fact]
    public void HandleDispatchesCreateAnalysisApiActivity()
    {
        var eventClient = new Mock<IApplicationActivityEngine>();

        var analysisStartedEvent = new AnalysisStartedEvent { AnalysisId = Guid.NewGuid() };
        analysisStartedEvent.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Dispatch(It.Is<CreateAnalysisApiActivity>(value =>
            value.CachedAnalysisId == analysisStartedEvent.AnalysisId
        )));
    }
}
