using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
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
        var repositoryId = "abcfe123";
        var commitId = "fecbec321";
        var asOfDate = new DateTimeOffset(2021, 12, 12, 10, 15, 25, 0, TimeSpan.Zero);
        var configuration = new Mock<IConfiguration>();
        var historyStopData = new HistoryStopData(configuration.Object, repositoryId, commitId, asOfDate);
        var agentExecutablePath = "/path/to/agent";
        var activityEngine = new Mock<IApplicationActivityEngine>();

        var package = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
        var packageEvent = new PackageFoundEvent
        {
            AnalysisId = analysisId,
            HistoryStopData = historyStopData,
            AgentExecutablePath = agentExecutablePath,
            Package = package
        };

        packageEvent.Handle(activityEngine.Object);

        activityEngine.Verify(mock => mock.Dispatch(It.Is<ComputeLibYearForPackageActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopData == historyStopData &&
            value.AgentExecutablePath == agentExecutablePath &&
            value.Package == package)));
    }
}
