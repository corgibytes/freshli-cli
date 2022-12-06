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
    [Fact]
    public async Task CorrectlyDispatchesGenerateBillOfMaterialsActivity()
    {
        const string manifestPath = "/path/to/manifest";
        var engine = new Mock<IApplicationActivityEngine>();

        const string agentExecutablePath = "/path/to/agent";
        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var manifestEvent =
            new ManifestDetectedEvent(analysisId, historyStopPointId, agentExecutablePath, manifestPath);
        await manifestEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<GenerateBillOfMaterialsActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.ManifestPath == manifestPath &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }

    [Fact]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var engine = new Mock<IApplicationActivityEngine>();

        var appEvent = new ManifestDetectedEvent(Guid.NewGuid(), 29, "/path/to/agent", "/path/to/manifest");

        var exception = new InvalidOperationException();
        engine.Setup(mock => mock.Dispatch(It.IsAny<GenerateBillOfMaterialsActivity>())).Throws(exception);

        await appEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
            value.HistoryStopPointId == appEvent.HistoryStopPointId &&
            value.Error == exception)));
    }
}
