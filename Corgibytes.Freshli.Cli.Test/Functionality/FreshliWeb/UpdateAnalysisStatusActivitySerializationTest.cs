using System;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class UpdateAnalysisStatusActivitySerializationTest : SerializationTest<UpdateAnalysisStatusActivity>
{
    protected override UpdateAnalysisStatusActivity BuildIncoming() =>
        new(Guid.NewGuid(), "example status");

    protected override void AssertEqual(UpdateAnalysisStatusActivity incoming, UpdateAnalysisStatusActivity outgoing)
    {
        Assert.Equal(incoming.ApiAnalysisId, outgoing.ApiAnalysisId);
        Assert.Equal(incoming.Status, outgoing.Status);
    }
}
