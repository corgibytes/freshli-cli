using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
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
    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task CorrectlyDispatchesGenerateBillOfMaterialsActivity()
    {
        const string manifestPath = "path/to/manifest";
        var engine = new Mock<IApplicationActivityEngine>();

        const string agentExecutablePath = "/path/to/agent";
        var parent = new Mock<IHistoryStopPointProcessingTask>();
        var cancellationToken = new System.Threading.CancellationToken(false);

        var cachedManifest = new CachedManifest
        {
            ManifestFilePath = manifestPath
        };

        var manifestEvent = new ManifestDetectedEvent
        {
            Parent = parent.Object,
            AgentExecutablePath = agentExecutablePath,
            Manifest = cachedManifest
        };
        await manifestEvent.Handle(engine.Object, cancellationToken);

        engine.Verify(
            mock => mock.Dispatch(
                It.Is<GenerateBillOfMaterialsActivity>(value =>
                    value.Parent == manifestEvent &&
                    value.AgentExecutablePath == agentExecutablePath
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var engine = new Mock<IApplicationActivityEngine>();
        var cancellationToken = new System.Threading.CancellationToken(false);
        var parent = new Mock<IHistoryStopPointProcessingTask>();

        var cachedManifest = new CachedManifest { ManifestFilePath = "path/to/manifest" };

        var appEvent = new ManifestDetectedEvent
        {
            Parent = parent.Object,
            AgentExecutablePath = "/path/to/agent",
            Manifest = cachedManifest
        };

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
                    value.Parent == appEvent &&
                    value.Error == exception
                ),
                cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
