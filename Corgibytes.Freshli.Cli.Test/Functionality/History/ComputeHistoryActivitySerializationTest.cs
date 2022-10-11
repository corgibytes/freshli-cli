using System;
using Corgibytes.Freshli.Cli.Functionality.History;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class ComputeHistoryActivitySerializationTest : SerializationTest<ComputeHistoryActivity>
{
    protected override ComputeHistoryActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            this.BuildHistoryStopData()
        );

    protected override void AssertEqual(ComputeHistoryActivity incoming, ComputeHistoryActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        this.AssertHistoryStopDataEqual(incoming.HistoryStopData, outgoing.HistoryStopData);
    }
}

