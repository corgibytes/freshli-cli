using System;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class CreateApiHistoryStopActivitySerializationTest : SerializationTest<CreateApiHistoryStopActivity>
{
    protected override CreateApiHistoryStopActivity BuildIncoming() =>
        new(
            Guid.NewGuid(),
            29
        );

    protected override void AssertEqual(CreateApiHistoryStopActivity incoming, CreateApiHistoryStopActivity outgoing)
    {
        Assert.Equal(incoming.CachedAnalysisId, outgoing.CachedAnalysisId);
        Assert.Equal(incoming.HistoryStopPointId, outgoing.HistoryStopPointId);
    }
}
