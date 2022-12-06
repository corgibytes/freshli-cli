using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class BillOfMaterialsGeneratedEventTest
{
    [Fact]
    public async Task CorrectlyDispatchesComputeLibYearActivity()
    {
        const string pathToBom = "/path/to/bom";
        const string agentExecutablePath = "/path/to/agent";

        var analysisId = Guid.NewGuid();
        const int historyStopPointId = 29;
        var billOfMaterialsGeneratedEvent =
            new BillOfMaterialsGeneratedEvent(analysisId, historyStopPointId, pathToBom, agentExecutablePath);

        var engine = new Mock<IApplicationActivityEngine>();

        await billOfMaterialsGeneratedEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<DeterminePackagesFromBomActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.PathToBom == pathToBom &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }

    [Fact]
    public async Task HandleCorrectlyDealsWithExceptions()
    {
        var appEvent = new BillOfMaterialsGeneratedEvent(Guid.NewGuid(), 29, "/path/to/bom", "/path/to/agent");

        var engine = new Mock<IApplicationActivityEngine>();

        var exception = new InvalidOperationException();
        engine.Setup(mock => mock.Dispatch(It.IsAny<DeterminePackagesFromBomActivity>())).Throws(exception);

        await appEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<FireHistoryStopPointProcessingErrorActivity>(value =>
            value.HistoryStopPointId == 29 &&
            value.Error == exception
        )));
    }
}
