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
            this.BuildHistoryStopData(),
            "/it/manifests"
        );

    protected override void AssertEqual(GenerateBillOfMaterialsActivity incoming, GenerateBillOfMaterialsActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
        this.AssertHistoryStopDataEqual(incoming.HistoryStopData, outgoing.HistoryStopData);
        Assert.Equal(incoming.ManifestPath, outgoing.ManifestPath);
    }
}

