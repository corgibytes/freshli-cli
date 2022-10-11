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
            this.BuildHistoryStopData()
        );

    protected override void AssertEqual(CreateApiHistoryStopActivity incoming, CreateApiHistoryStopActivity outgoing)
    {
        Assert.Equal(incoming.CachedAnalysisId, outgoing.CachedAnalysisId);
        this.AssertHistoryStopDataEqual(incoming.HistoryStopData, outgoing.HistoryStopData);
    }
}
