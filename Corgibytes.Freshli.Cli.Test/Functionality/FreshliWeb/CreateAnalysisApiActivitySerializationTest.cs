using System;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.FreshliWeb;

[IntegrationTest]
// ReSharper disable once UnusedType.Global
public class CreateAnalysisApiActivitySerializationTest : SerializationTest<CreateAnalysisApiActivity>
{
    protected override CreateAnalysisApiActivity BuildIncoming() => new(Guid.NewGuid());

    protected override void AssertEqual(CreateAnalysisApiActivity incoming, CreateAnalysisApiActivity outgoing)
    {
        Assert.Equal(incoming.CachedAnalysisId, outgoing.CachedAnalysisId);
    }
}
