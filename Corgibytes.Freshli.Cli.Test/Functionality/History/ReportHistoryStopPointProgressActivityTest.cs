using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[UnitTest]
public class ReportHistoryStopPointProgressActivityTest
{
    [Fact(Timeout = 500)]
    public async Task HandleWhenOperationIsCompleted()
    {
        var eventClient = new Mock<IApplicationEventEngine>();

        eventClient.Setup(mock => mock.AreOperationsPending(It.IsAny<Func<IHistoryStopPointProcessingTask, bool>>()))
            .ReturnsAsync(false);

        var activity = new ReportHistoryStopPointProgressActivity { HistoryStopPointId = 12 };

        await activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<HistoryStopPointProcessingCompletedEvent>(value =>
            value.HistoryStopPointId == activity.HistoryStopPointId)));
    }

    [Fact(Timeout = 500)]
    public async Task HandleWhenOperationIsStillInProgress()
    {
        var eventClient = new Mock<IApplicationEventEngine>();

        eventClient.Setup(mock => mock.AreOperationsPending(It.IsAny<Func<IHistoryStopPointProcessingTask, bool>>()))
            .ReturnsAsync(true);

        var activity = new ReportHistoryStopPointProgressActivity { HistoryStopPointId = 12 };

        await activity.Handle(eventClient.Object);

        eventClient.Verify(mock => mock.Fire(It.Is<HistoryStopPointProcessingCompletedEvent>(value =>
            value.HistoryStopPointId == activity.HistoryStopPointId)), Times.Never);
    }
}
