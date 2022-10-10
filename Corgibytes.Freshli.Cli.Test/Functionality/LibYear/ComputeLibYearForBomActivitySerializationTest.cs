using System;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

public class ComputeLibYearForBomActivitySerializationTest : SerializationTest<ComputeLibYearForBomActivity>
{
    protected override ComputeLibYearForBomActivity BuildIncoming()
    {
        return new ComputeLibYearForBomActivity(
            Guid.NewGuid(),
            this.BuildHistoryStopData(),
            "/path/to/bom",
            "/path/to/agent"
        );
    }

    protected override void AssertEqual(ComputeLibYearForBomActivity incoming, ComputeLibYearForBomActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        this.AssertHistoryStopDataEqual(incoming.HistoryStopData, outgoing.HistoryStopData);
        Assert.Equal(incoming.PathToBom, outgoing.PathToBom);
        Assert.Equal(incoming.AgentExecutablePath, outgoing.AgentExecutablePath);
    }
}
