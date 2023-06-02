using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[UnitTest]
public class PackageFoundEventTest
{
    [Fact(Timeout = 500)]
    public async Task HandleCorrectlyDispatchesComputeLibYearForPackageActivity()
    {
        var analysisId = Guid.NewGuid();
        const string agentExecutablePath = "/path/to/agent";
        var activityEngine = new Mock<IApplicationActivityEngine>();
        var cancellationToken = new System.Threading.CancellationToken(false);
        var parent = new Mock<IHistoryStopPointProcessingTask>();

        var package = new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6");
        var packageEvent = new PackageFoundEvent
        {
            AnalysisId = analysisId,
            Parent = parent.Object,
            AgentExecutablePath = agentExecutablePath,
            Package = package
        };

        await packageEvent.Handle(activityEngine.Object, cancellationToken);

        activityEngine.Verify(
            mock => mock.Dispatch(
                It.Is<ComputeLibYearForPackageActivity>(value =>
                    value.AnalysisId == analysisId &&
                    value.Parent == parent.Object &&
                    value.AgentExecutablePath == agentExecutablePath &&
                    value.Package == package
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
