using System;
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
    public void HandleCorrectlyDispatchesComputeLibYearForPackageActivity()
    {
        var analysisId = Guid.NewGuid();
        var agentExecutablePath = "/path/to/agent";
        var activityEngine = new Mock<IApplicationActivityEngine>();

        var historyStopPointId = 29;
        var package = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
        var packageEvent = new PackageFoundEvent
        {
            AnalysisId = analysisId,
            HistoryStopPointId = historyStopPointId,
            AgentExecutablePath = agentExecutablePath,
            Package = package
        };

        packageEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock => mock.Dispatch(It.Is<ComputeLibYearForPackageActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.AgentExecutablePath == agentExecutablePath &&
            value.Package == package)));
    }
}
