using System;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class GenerateBillOfMaterialsActivitySerializationTest : SerializationTest<GenerateBillOfMaterialsActivity>
{
    protected override GenerateBillOfMaterialsActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            "/agent/brown",
            29,
            "/it/manifests"
        );

    protected override void AssertEqual(GenerateBillOfMaterialsActivity incoming,
        GenerateBillOfMaterialsActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
        Assert.Equal(incoming.ManifestPath, outgoing.ManifestPath);
    }
}
