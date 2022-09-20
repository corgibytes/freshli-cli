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
            Url = "something",
            Branch = "somethingelse",
            CacheDir = "/path/to/cache",
            GitPath = "/git"
        };

        var engine = new Mock<IApplicationActivityEngine>();

        apiEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<CloneGitRepositoryActivity>(value =>
            value.AnalysisId == apiEvent.CachedAnalysisId &&
            value.RepoUrl == apiEvent.Url &&
            value.Branch == apiEvent.Branch &&
            value.CacheDir == apiEvent.CacheDir &&
            value.GitPath == apiEvent.GitPath
        )));

    }
}
