using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[UnitTest]
public class ApiPackageLibYearCreatedEventTest
{
    [Fact(Timeout = 500)]
    public async Task Handle()
    {
        var activityClient = new Mock<IApplicationActivityEngine>();

        var appEvent = new ApiPackageLibYearCreatedEvent { HistoryStopPointId = 12 };

        await appEvent.Handle(activityClient.Object);

        activityClient.Verify(mock => mock.Dispatch(It.Is<ReportHistoryStopPointProgressActivity>(
            value => value.HistoryStopPointId == appEvent.HistoryStopPointId)));
    }

    [Fact(Timeout = 500)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var activityClient = new Mock<IApplicationActivityEngine>();

        var appEvent = new ApiPackageLibYearCreatedEvent { HistoryStopPointId = 12 };

        var exception = new InvalidOperationException();
        activityClient.Setup(mock => mock.Dispatch(It.IsAny<ReportHistoryStopPointProgressActivity>()))
            .Throws(exception);

        await appEvent.Handle(activityClient.Object);

        activityClient.Verify(mock => mock.Dispatch(It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
            value.HistoryStopPointId == appEvent.HistoryStopPointId &&
            value.Error == exception)));
    }
}
