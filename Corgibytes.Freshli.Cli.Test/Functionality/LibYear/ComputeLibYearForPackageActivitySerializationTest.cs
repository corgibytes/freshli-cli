using System;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class ComputeLibYearForPackageActivitySerializationTest : SerializationTest<ComputeLibYearForPackageActivity>
{
    protected override ComputeLibYearForPackageActivity BuildIncoming() =>
        new()
        {
            AgentExecutablePath = "/path/to/agent",
            AnalysisId = Guid.NewGuid(),
            HistoryStopData = this.BuildHistoryStopData(),
            Package = new PackageURL(
                "pkg:maven/org.apache.xmlgraphics/batik-anim@1.9.1?repository_url=repo.spring.io%2Frelease")
        };

    protected override void AssertEqual(ComputeLibYearForPackageActivity incoming,
        ComputeLibYearForPackageActivity outgoing)
    {
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        this.AssertHistoryStopDataEqual(incoming.HistoryStopData, outgoing.HistoryStopData);
        Assert.Equal(incoming.Package.ToString(), outgoing.Package.ToString());
    }
}
