using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class CheckoutHistoryActivityTest
{
    [Fact]
    public void Handle()
    {
        var commitId = "abcdef1";
        var gitExecutablePath = "/path/to/git";
        var repositoryId = Guid.NewGuid().ToString();
        var cacheDirectory = "/path/to/cache/dir";
        var archiveLocation = $"{cacheDirectory}/histories/{repositoryId}/{commitId}";

        var gitManager = new Mock<IGitManager>();

        var activity = new CheckoutHistoryActivity(
            gitManager.Object, gitExecutablePath, cacheDirectory, repositoryId, commitId);

        var eventEngine = new Mock<IApplicationEventEngine>();

        var parsedCommitId = new GitCommitIdentifier(commitId);
        gitManager.Setup(mock => mock.ParseCommitId(commitId)).Returns(parsedCommitId);
        gitManager.Setup(
            mock => mock.CreateArchive(repositoryId, cacheDirectory, parsedCommitId, gitExecutablePath)
        ).Returns(archiveLocation);

        activity.Handle(eventEngine.Object);

        eventEngine.Verify(
            mock => mock.Fire(It.Is<HistoryStopCheckedOutEvent>(
                appEvent => appEvent.AnalysisLocation.Path == archiveLocation)));
    }
}
