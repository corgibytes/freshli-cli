using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
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

        var analysisStartedEvent = new AnalysisStartedEvent()
        {
            CacheDir = "/path/to/cache/dir",
            GitPath = "/path/to/git",
            AnalysisId = Guid.NewGuid()
        };
        analysisStartedEvent.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Dispatch(It.Is<CreateAnalysisApiActivity>(value =>
            value.CacheDir == analysisStartedEvent.CacheDir &&
            value.GitPath == analysisStartedEvent.GitPath &&
            value.CachedAnalysisId == analysisStartedEvent.AnalysisId
        )));
    }
}
