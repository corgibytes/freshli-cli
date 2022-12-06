using System;
using System.Threading.Tasks;
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
    [Fact]
    public async Task Handle()
    {
        var analysisId = Guid.NewGuid();
        var historyStopPointId = 12;

        var progressReporter = new Mock<IAnalyzeProgressReporter>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(IAnalyzeProgressReporter)))
            .Returns(progressReporter.Object);
        var activityEngine = new Mock<IApplicationActivityEngine>();
        activityEngine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        var logger = new Mock<ILogger<HistoryStopCheckedOutEvent>>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<HistoryStopCheckedOutEvent>)))
            .Returns(logger.Object);

        var appEvent = new HistoryStopCheckedOutEvent
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId
        };

        await appEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock => mock.Dispatch(It.Is<DetectAgentsForDetectManifestsActivity>(value =>
            value.AnalysisId == analysisId && value.HistoryStopPointId == historyStopPointId)));

        progressReporter.Verify(mock =>
            mock.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Archive));
    }
}
