using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Json.Schema;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class HistoryStopPointProcessingCompletedEventTest
{
    [Fact(Timeout = 500)]
    public async Task Handle()
    {
        var activityClient = new Mock<IApplicationActivityEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        var progressReporter = new Mock<IAnalyzeProgressReporter>();

        activityClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(IAnalyzeProgressReporter)))
            .Returns(progressReporter.Object);
        var logger = new Mock<ILogger<HistoryStopPointProcessingCompletedEvent>>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<HistoryStopPointProcessingCompletedEvent>)))
            .Returns(logger.Object);

        var appEvent = new HistoryStopPointProcessingCompletedEvent { HistoryStopPointId = 12 };

        await appEvent.Handle(activityClient.Object);

        progressReporter.Verify(mock =>
            mock.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Process));
    }

}
