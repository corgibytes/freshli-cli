using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test.Services;

[IntegrationTest]
public class AgentManagerTest
{
    [Fact]
    public void GetReader()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<AgentReader>)))
            .Returns(NullLogger<AgentReader>.Instance);
        using var manager = new AgentManager(
            new CacheManager(new Configuration(new Environment())),
            new NullLogger<AgentManager>(),
            new Configuration(new Environment()),
            serviceProvider.Object
        );

        var reader = manager.GetReader("freshli-agent-java");

        Assert.NotNull(reader);
    }
}
