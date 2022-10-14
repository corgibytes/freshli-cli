using System;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class BillOfMaterialsGeneratedEventSerializationTest : SerializationTest<BillOfMaterialsGeneratedEvent>
{
    protected override BillOfMaterialsGeneratedEvent BuildIncoming() =>
        new(Guid.NewGuid(), 29, "/path/to/bom", "/path/to/agent");

    protected override void AssertEqual(BillOfMaterialsGeneratedEvent incoming, BillOfMaterialsGeneratedEvent outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
        Assert.Equal(incoming.PathToBillOfMaterials, outgoing.PathToBillOfMaterials);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
    }
}
