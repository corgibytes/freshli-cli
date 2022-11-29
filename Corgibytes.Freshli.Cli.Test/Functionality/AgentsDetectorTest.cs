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
    public void TestingAgentsDetectWithMacAndLinuxPaths()
    {
        var executableFinder = new Mock<IExecutableFinder>();
        executableFinder
            .Setup(mock => mock.GetExecutables())
            .Returns(new List<string>
            {
                "/usr/bin/freshli-agent-java",
                "/usr/local/bin/freshli-agent-dotnet",
                "/home/user/bin/utility"
            });

        var environment = new Mock<IEnvironment>();
        environment
            .Setup(mock => mock.PathSeparator)
            .Returns("/");

        var agentsDetector = new AgentsDetector(executableFinder.Object, environment.Object);
        var expectedResults = new List<string>
        {
            "/usr/bin/freshli-agent-java",
            "/usr/local/bin/freshli-agent-dotnet"
        };
        var results = agentsDetector.Detect();
        Assert.Equal(expectedResults, results);
    }

    [Fact]
    public void TestingAgentsDetectWithWindowsPaths()
    {
        var executableFinder = new Mock<IExecutableFinder>();
        executableFinder
            .Setup(mock => mock.GetExecutables())
            .Returns(new List<string>
            {
                "C:\\Agents\\freshli-agent-java.bat",
                "D:\\Agents\\dotnet\\bin\\freshli-agent-dotnet.exe",
                "E:\\Utilities\\bin\\utility.com"
            });

        var environment = new Mock<IEnvironment>();
        environment
            .Setup(mock => mock.PathSeparator)
            .Returns("\\");

        var agentsDetector = new AgentsDetector(executableFinder.Object, environment.Object);
        var expectedResults = new List<string>
        {
            "C:\\Agents\\freshli-agent-java.bat",
            "D:\\Agents\\dotnet\\bin\\freshli-agent-dotnet.exe"
        };
        var results = agentsDetector.Detect();
        Assert.Equal(expectedResults, results);
    }
}
