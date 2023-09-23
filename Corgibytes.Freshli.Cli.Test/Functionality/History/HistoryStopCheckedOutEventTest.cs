using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class HistoryStopCheckedOutEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task Handle()
    {
        var progressReporter = new Mock<IAnalyzeProgressReporter>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAnalyzeProgressReporter)))
            .Returns(progressReporter.Object);
        var activityEngine = new Mock<IApplicationActivityEngine>();
        activityEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        var logger = new Mock<ILogger<HistoryStopCheckedOutEvent>>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<HistoryStopCheckedOutEvent>)))
            .Returns(logger.Object);

        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPoint).Returns(new CachedHistoryStopPoint { Id = 29 });
        var appEvent = new HistoryStopCheckedOutEvent
        {
            Parent = parent.Object
        };

        var cancellationToken = new System.Threading.CancellationToken(false);
        await appEvent.Handle(activityEngine.Object, cancellationToken);

        activityEngine.Verify(
            mock => mock.Dispatch(
                It.Is<DetectAgentsForDetectManifestsActivity>(value =>
                    value.Parent == appEvent
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        progressReporter.Verify(mock =>
            mock.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Archive));
    }
}
