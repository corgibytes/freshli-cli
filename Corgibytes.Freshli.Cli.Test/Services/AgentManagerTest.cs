using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Services;

[IntegrationTest]
public class AgentManagerTest
{
    [Fact]
    public void GetReader()
    {
        var manager = new AgentManager(new CommandInvoker());

        var reader = manager.GetReader("freshli-agent-java");

        Assert.Equal("freshli-agent-java", reader.AgentExecutablePath);
    }
}
