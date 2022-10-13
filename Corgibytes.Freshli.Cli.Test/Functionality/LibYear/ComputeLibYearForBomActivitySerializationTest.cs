using System;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class ComputeLibYearForBomActivitySerializationTest : SerializationTest<DeterminePackagesFromBomActivity>
{
    protected override DeterminePackagesFromBomActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            29,
            "/path/to/bom",
            "/path/to/agent"
        );

    protected override void AssertEqual(DeterminePackagesFromBomActivity incoming, DeterminePackagesFromBomActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
        Assert.Equal(incoming.PathToBom, outgoing.PathToBom);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
    }
}
