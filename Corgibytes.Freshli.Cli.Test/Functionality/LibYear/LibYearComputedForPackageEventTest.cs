using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class LibYearComputedForPackageEventTest
{
    [Fact]
    public async Task HandleCorrectlyDispatchesCreateApiPackageLibYear()
    {
        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 12;
        const int packageLibYearId = 9;
        const string agentExecutablePath = "/path/to/agent";

        var appEvent = new LibYearComputedForPackageEvent
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            PackageLibYearId = packageLibYearId,
            AgentExecutablePath = agentExecutablePath
        };

        var activityClient = new Mock<IApplicationActivityEngine>();

        await appEvent.Handle(activityClient.Object);

        activityClient.Verify(mock => mock.Dispatch(It.Is<CreateApiPackageLibYearActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.PackageLibYearId == packageLibYearId &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }

    [Fact]
    public async Task HandleDispatchesFireHistoryStopPointProcessingErrorActivity()
    {
        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 12;
        const int packageLibYearId = 9;
        const string agentExecutablePath = "/path/to/agent";

        var appEvent = new LibYearComputedForPackageEvent
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            PackageLibYearId = packageLibYearId,
            AgentExecutablePath = agentExecutablePath
        };

        var activityClient = new Mock<IApplicationActivityEngine>();

        var exception = new InvalidOperationException();
        activityClient.Setup(mock => mock.Dispatch(It.IsAny<CreateApiPackageLibYearActivity>())).Throws(exception);

        await appEvent.Handle(activityClient.Object);

        activityClient.Verify(mock => mock.Dispatch(It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
            value.HistoryStopPointId == appEvent.HistoryStopPointId &&
            value.Error == exception)));
    }
}
