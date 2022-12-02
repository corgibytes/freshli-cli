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
    [Fact]
    public async Task Handle()
    {
        var activityClient = new Mock<IApplicationActivityEngine>();

        var appEvent = new ApiPackageLibYearCreatedEvent { HistoryStopPointId = 12 };

        await appEvent.Handle(activityClient.Object);

        activityClient.Verify(mock => mock.Dispatch(It.Is<ReportHistoryStopPointProgressActivity>(
            value => value.HistoryStopPointId == appEvent.HistoryStopPointId)));
    }
}
