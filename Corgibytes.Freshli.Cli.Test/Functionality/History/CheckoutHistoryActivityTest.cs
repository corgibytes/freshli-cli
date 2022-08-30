using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Test;
using Moq;
using Xunit;

namespace Corgibytes.Freshil.Cli.Test.Functionality.History;

[UnitTest]
public class CheckoutHistoryActivityTest
{
    [Fact]
    public void Handle()
    {
        var commitSha = "abcdef1";
        var gitExecutablePath = "/path/to/git";
        var repositoryId = Guid.NewGuid().ToString();
        var cacheDirectory = "/path/to/cache/dir";
        var archiveLocation = "/path/to/archive/location";

        var gitManager = new Mock<IGitManager>();

        var activity = new CheckoutHistoryActivity(
            gitManager.Object, gitExecutablePath, cacheDirectory, repositoryId, commitSha);

        var eventEngine = new Mock<IApplicationEventEngine>();

        var parsedCommitSha = new GitCommitIdentifier(commitSha);
        gitManager.Setup(mock => mock.ParseCommitSha(commitSha)).Returns(parsedCommitSha);
        gitManager.Setup(
            mock => mock.CreateArchive(repositoryId, cacheDirectory, parsedCommitSha, gitExecutablePath)
        ).Returns(archiveLocation);

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(
            mock => mock.Fire(It.Is<HistoryStopCheckedOutEvent>(
                appEvent => appEvent.AnalysisLocation.Path == archiveLocation)));
    }
}
