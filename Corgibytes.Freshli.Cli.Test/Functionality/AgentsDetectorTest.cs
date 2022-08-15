using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[UnitTest]
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

    [Fact]
    public void DetectHandlesDuplicateAgentNames()
    {
        var environment = new Mock<IEnvironment>();

        environment.Setup(mock => mock.DirectoriesInSearchPath).Returns(
            new List<string>
            {
                "/usr/local/bin",
                "/bin"
            }
        );

        // Intentionally include the same file name in different directories
        environment.Setup(mock => mock.GetListOfFiles("/bin")).Returns(new List<string?> {"freshli-agent-java"});
        environment.Setup(mock => mock.GetListOfFiles("/usr/local/bin")).Returns(new List<string?> {"freshli-agent-java"});

        var agentsDetector = new AgentsDetector(environment.Object);

        // The only `freshli-agent-java` command that should be included in the output is the directory that comes
        // first in the list of directories in the search path.
        var expectedResults = new List<string>
        {
            "/usr/local/bin/freshli-agent-java"
        };
        var results = agentsDetector.Detect();
        Assert.Equal(expectedResults, results);
    }
}
