using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[UnitTest]
public class GitRepositoryCloneStartedEventTest
{
    [Fact(Timeout = 500)]
    public async Task HandleSignalsProgressReporter()
    {
        var analysisId = Guid.NewGuid();
        var activityClient = new Mock<IApplicationActivityEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        var progressReporter = new Mock<IAnalyzeProgressReporter>();

        serviceProvider.Setup(mock => mock.GetService(typeof(IAnalyzeProgressReporter)))
            .Returns(progressReporter.Object);
        activityClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var appEvent = new GitRepositoryCloneStartedEvent { AnalysisId = analysisId };
        await appEvent.Handle(activityClient.Object);

        progressReporter.Verify(mock => mock.ReportGitOperationStarted(GitOperation.CreateNewClone));
    }
}
