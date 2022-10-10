using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
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
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(mock => mock.CacheDir).Returns("/cache/directory");
        var historyStopData = new HistoryStopData(configuration.Object, "2dbc2fd2358e1ea1b7a6bc08ea647b9a337ac92d",
            "da39a3ee5e6b4b0d3255bfef95601890afd80709");
        var pathToBom = "/path/to/bom";
        var agentExecutablePath = "/path/to/agent";

        var analysisId = Guid.NewGuid();
        var billOfMaterialsGeneratedEvent =
            new BillOfMaterialsGeneratedEvent(analysisId, historyStopData, pathToBom, agentExecutablePath);

        serviceProvider.Setup(mock => mock.GetService(typeof(ICalculateLibYearFromFile)))
            .Returns(calculateLibYearFromFile.Object);

        var engine = new Mock<IApplicationActivityEngine>();
        engine.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);

        billOfMaterialsGeneratedEvent.Handle(engine.Object);

        engine.Verify(mock => mock.Dispatch(It.Is<ComputeLibYearForBomActivity>(value =>
            value.AnalysisId == analysisId &&
            value.HistoryStopData == historyStopData &&
            value.PathToBom == pathToBom &&
            value.AgentExecutablePath == agentExecutablePath
        )));
    }
}
