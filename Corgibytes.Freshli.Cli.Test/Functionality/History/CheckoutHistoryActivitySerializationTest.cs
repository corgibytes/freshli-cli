using System;
using Corgibytes.Freshli.Cli.Functionality.History;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class CheckoutHistoryActivitySerializationTest : SerializationTest<CheckoutHistoryActivity>
{
    protected override CheckoutHistoryActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            this.BuildHistoryStopData()
        );

    protected override void AssertEqual(CheckoutHistoryActivity incoming, CheckoutHistoryActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        this.AssertHistoryStopDataEqual(incoming.HistoryStopData, outgoing.HistoryStopData);
    }
}
