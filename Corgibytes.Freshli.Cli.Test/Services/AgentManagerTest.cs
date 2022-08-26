using Corgibytes.Freshli.Cli.Services;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Services;

[UnitTest]
public class AgentManagerTest
{
    [Fact]
    public void GetReader()
    {
        var manager = new AgentManager();

        var reader = manager.GetReader("freshli-agent-java");

        Assert.Equal("freshli-agent-java", reader.AgentExecutablePath);
    }
}
