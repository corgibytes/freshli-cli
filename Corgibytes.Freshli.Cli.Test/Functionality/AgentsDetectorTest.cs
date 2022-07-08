using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class AgentsDetectorTest
{
    [Fact]
    public void TestingAgentsDetect()
    {
        var agentsDetector = new AgentsDetector(new MockEnvironment());
        var expectedResults = new List<string>
        {
            "/usr/local/bin/freshli-agent-java",
            "/usr/local/bin/freshli-agent-javascript",
            "/usr/local/agents/bin/freshli-agent-csharp",
            "/home/freshli-user/bin/agents/freshli-agent-ruby"
        };
        var results = agentsDetector.Detect();
        Assert.Equal(expectedResults, results);
    }
}
