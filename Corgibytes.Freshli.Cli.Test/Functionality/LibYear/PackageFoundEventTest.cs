using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class PackageFoundEventTest
{
    [Fact]
    public async Task HandleCorrectlyDispatchesComputeLibYearForPackageActivity()
    {
        var analysisId = Guid.NewGuid();
        const string agentExecutablePath = "/path/to/agent";
        var activityEngine = new Mock<IApplicationActivityEngine>();

        const int historyStopPointId = 29;
        var package = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
        var packageEvent = new PackageFoundEvent
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            AgentExecutablePath = agentExecutablePath,
            Package = package
        };

        await packageEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock => mock.Dispatch(It.Is<ComputeLibYearForPackageActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.AgentExecutablePath == agentExecutablePath &&
            value.Package == package)));
    }
}
