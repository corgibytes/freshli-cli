using System;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class CreateApiPackageLibYearActivitySerializationTest : SerializationTest<CreateApiPackageLibYearActivity>
{
    protected override CreateApiPackageLibYearActivity BuildIncoming() => new()
    {
        AnalysisId = Guid.NewGuid(),
        HistoryStopPointId = 12,
        PackageLibYearId = 9,
        AgentExecutablePath = "/path/to/agent"
    };

    protected override void AssertEqual(CreateApiPackageLibYearActivity incoming,
        CreateApiPackageLibYearActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
        Assert.Equal(incoming.PackageLibYearId, outgoing.PackageLibYearId);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
    }
}
