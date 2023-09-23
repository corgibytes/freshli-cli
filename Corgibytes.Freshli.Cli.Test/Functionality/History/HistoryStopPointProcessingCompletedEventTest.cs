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
public class HistoryStopPointProcessingCompletedEventTest
{
    [Fact(Timeout = Constants.DefaultTestTimeout)]
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

        var parent = new Mock<IHistoryStopPointProcessingTask>();
        parent.Setup(mock => mock.HistoryStopPoint).Returns(new CachedHistoryStopPoint { Id = 29 });
        var appEvent = new HistoryStopPointProcessingCompletedEvent { Parent = parent.Object };

        var cancellationToken = new System.Threading.CancellationToken(false);
        await appEvent.Handle(activityClient.Object, cancellationToken);

        progressReporter.Verify(mock =>
            mock.ReportSingleHistoryStopPointOperationFinished(HistoryStopPointOperation.Process));
    }

}
