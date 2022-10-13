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
        AgentExecutablePath = "/path/to/agent",
        PackageLibYear = this.BuildPackageLibYear()
    };

    protected override void AssertEqual(CreateApiPackageLibYearActivity incoming, CreateApiPackageLibYearActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
        this.AssertPackageLibYearEqual(incoming.PackageLibYear, outgoing.PackageLibYear);
    }
}
