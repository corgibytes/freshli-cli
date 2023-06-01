using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class ManifestDetectedEventTest
{
    [Fact(Timeout = 500)]
    public async Task CorrectlyDispatchesGenerateBillOfMaterialsActivity()
    {
        const string manifestPath = "/path/to/manifest";
        var engine = new Mock<IApplicationActivityEngine>();

        const string agentExecutablePath = "/path/to/agent";
        var analysisId = Guid.NewGuid();
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        var cancellationToken = new System.Threading.CancellationToken(false);
        var manifestEvent =
            new ManifestDetectedEvent(analysisId, parent.Object, agentExecutablePath, manifestPath);
        await manifestEvent.Handle(engine.Object, cancellationToken);

        engine.Verify(
            mock => mock.Dispatch(
                It.Is<GenerateBillOfMaterialsActivity>(value =>
                    value.AnalysisId == analysisId &&
                    value.Parent == parent.Object &&
                    value.ManifestPath == manifestPath &&
                    value.AgentExecutablePath == agentExecutablePath
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = 500)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var engine = new Mock<IApplicationActivityEngine>();
        var cancellationToken = new System.Threading.CancellationToken(false);
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        var appEvent = new ManifestDetectedEvent(Guid.NewGuid(), parent.Object, "/path/to/agent", "/path/to/manifest");

        var exception = new InvalidOperationException();
        engine.Setup(
            mock => mock.Dispatch(
                It.IsAny<GenerateBillOfMaterialsActivity>(),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        ).Throws(exception);

        await appEvent.Handle(engine.Object, cancellationToken);

        engine.Verify(
            mock => mock.Dispatch(
                It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
                    value.Parent == appEvent.Parent &&
                    value.Error == exception
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
