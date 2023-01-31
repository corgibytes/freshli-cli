using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Services;

[IntegrationTest]
public class AgentManagerTest
{
    [Fact]
    public void GetReader()
    {
        using var manager = new AgentManager(
            new CacheManager(new Configuration(new Environment())),
            new NullLogger<AgentManager>(),
            new Configuration(new Environment())
        );

        var reader = manager.GetReader("freshli-agent-java");

        Assert.NotNull(reader);
    }
}
