using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[UnitTest]
public class GitRepositoryClonedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CorrectlyDispatchesComputeHistoryActivity()
    {
        const string gitPath = "test";
        const string cacheDir = "example";
        const string gitRepositoryId = "example";
        var analysisId = Guid.NewGuid();
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(mock => mock.GitPath).Returns(gitPath);
        configuration.Setup(mock => mock.CacheDir).Returns(cacheDir);
        var historyStopData = new HistoryStopData
        {
            Configuration = configuration.Object,
            RepositoryId = gitRepositoryId
        };

        var clonedEvent = new GitRepositoryClonedEvent
        {
            AnalysisId = analysisId,
            HistoryStopData = historyStopData
        };

        var engine = new Mock<IApplicationActivityEngine>();

        var serviceProvider = new Mock<IServiceProvider>();
        var progressReporter = new Mock<IAnalyzeProgressReporter>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAnalyzeProgressReporter)))
            .Returns(progressReporter.Object);
        engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        var cancellationToken = new CancellationToken(false);
        await clonedEvent.Handle(engine.Object, cancellationToken);

        // Verify that it dispatches ComputeHistoryActivity
        engine.Verify(mock =>
            mock.Dispatch(
                It.Is<ComputeHistoryActivity>(value =>
                    value.AnalysisId == analysisId &&
                    value.HistoryStopData == historyStopData
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
        progressReporter.Verify(mock => mock.ReportGitOperationFinished(GitOperation.CreateNewClone));
    }
}
