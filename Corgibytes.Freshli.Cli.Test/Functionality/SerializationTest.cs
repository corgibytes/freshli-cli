using Hangfire.Common;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public abstract class SerializationTest
{
}

public abstract class SerializationTest<T> : SerializationTest
{
    [Fact]
    public void SerializeAndDeserialize()
    {
        var incoming = BuildIncoming();

        var data = SerializationHelper.Serialize(incoming);
        var outgoing = SerializationHelper.Deserialize<T>(data);

        AssertEqual(incoming, outgoing);
    }

    protected abstract T BuildIncoming();
    protected abstract void AssertEqual(T incoming, T outgoing);
}
