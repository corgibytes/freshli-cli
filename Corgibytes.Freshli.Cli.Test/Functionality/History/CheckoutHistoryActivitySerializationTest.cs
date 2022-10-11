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
            29
        );

    protected override void AssertEqual(CheckoutHistoryActivity incoming, CheckoutHistoryActivity outgoing)
    {
        Assert.Equal(incoming.AnalysisId, outgoing.AnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
    }
}
