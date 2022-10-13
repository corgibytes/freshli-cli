using System;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class BillOfMaterialsGeneratedEventTest
{
    [Fact]
    public void CorrectlyDispatchesComputeLibYearActivity()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var pathToBom = "/path/to/bom";
        var agentExecutablePath = "/path/to/agent";

        var analysisId = Guid.NewGuid();
        var historyStopPointId = 29;
        var billOfMaterialsGeneratedEvent =
            new BillOfMaterialsGeneratedEvent(analysisId, historyStopPointId, pathToBom, agentExecutablePath);

        var engine = new Mock<IApplicationActivityEngine>();
        engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        billOfMaterialsGeneratedEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<DeterminePackagesFromBomActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.PathToBom == pathToBom &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }
}
