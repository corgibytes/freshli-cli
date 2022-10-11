using System;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class BillOfMaterialsGeneratedEventTest
{
    [Fact]
    public void CorrectlyDispatchesComputeLibYearActivity()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var calculateLibYearFromFile = new Mock<ICalculateLibYearFromFile>();
        var pathToBom = "/path/to/bom";
        var agentExecutablePath = "/path/to/agent";

        var analysisId = Guid.NewGuid();
        var historyStopPointId = 29;
        var billOfMaterialsGeneratedEvent =
            new BillOfMaterialsGeneratedEvent(analysisId, historyStopPointId, pathToBom, agentExecutablePath);

        serviceProvider.Setup(mock => mock.GetService(typeof(ICalculateLibYearFromFile)))
            .Returns(calculateLibYearFromFile.Object);

        var engine = new Mock<IApplicationActivityEngine>();
        engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        billOfMaterialsGeneratedEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<ComputeLibYearForBomActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopPointId == historyStopPointId &&
            value.PathToBom == pathToBom &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }
}
