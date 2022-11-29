using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[UnitTest]
public class ExecutableFinderTest
{
    [Fact]
    public void FindHonorsExecutableStatusOnMacAndLinuxAndCorrectlyHandlesDuplicates()
    {
        var environment = new Mock<IEnvironment>();
        environment.Setup(mock => mock.IsWindows).Returns(false);
        environment.Setup(mock => mock.PathSeparator).Returns("/");

        environment.Setup(mock => mock.HomeDirectory).Returns("/home/user");
        environment
            .Setup(mock => mock.DirectoriesInSearchPath)
            .Returns(new List<string> { "/usr/bin", "/usr/local/bin", "~/bin" });

        environment
            .Setup(mock => mock.GetListOfFiles("/usr/bin"))
            .Returns(new List<string>
            {
                "freshli-agent-java",
                "freshli-agent-java.bat",
                "freshli-agent-dotnet",
                "freshli-agent-dotnet.bat",
                "other-one"
            });

        environment
            .Setup(mock => mock.GetListOfFiles("/usr/local/bin"))
            .Returns(new List<string>
            {
                "freshli-agent-java",
                "freshli-agent-java.bat",
                "freshli-agent-ruby",
                "freshli-agent-ruby.bat",
                "other-two"
            });

        environment
            .Setup(mock => mock.GetListOfFiles("/home/user/bin"))
            .Returns(new List<string>
            {
                "utility"
            });

        var executableStats = new Dictionary<string, bool>
        {
            { "/usr/bin/freshli-agent-java", true },
            { "/usr/bin/freshli-agent-java.bat", false },
            { "/usr/bin/freshli-agent-dotnet", true },
            { "/usr/bin/freshli-agent-dotnet.bat", false },
            { "/usr/bin/other-one", false },
            { "/usr/local/bin/freshli-agent-java", true },
            { "/usr/local/bin/freshli-agent-java.bat", false },
            { "/usr/local/bin/freshli-agent-ruby", true },
            { "/usr/local/bin/freshli-agent-ruby.bat", false },
            { "/usr/local/bin/other-two", false },
            { "/home/user/bin/utility", true }
        };

        foreach (var entry in executableStats)
        {
            environment
                .Setup(mock => mock.HasExecutableBit(entry.Key))
                .Returns(entry.Value);
        }

        var expectedExecutables = new List<string>
        {
            "/home/user/bin/utility",
            "/usr/bin/freshli-agent-dotnet",
            "/usr/bin/freshli-agent-java",
            "/usr/local/bin/freshli-agent-ruby"
        };

        var finder = new ExecutableFinder(environment.Object);

        var actualExecutables = finder.GetExecutables();

        Assert.Equal(expectedExecutables, actualExecutables);
    }

    [Fact]
    public void FindHonorsExecutableStatusOnWindowsAndCorrectlyHandlesDuplicates()
    {
        var environment = new Mock<IEnvironment>();
        environment.Setup(mock => mock.IsWindows).Returns(true);
        environment.Setup(mock => mock.PathSeparator).Returns("\\");

        environment.Setup(mock => mock.HomeDirectory).Returns("C:\\Users\\user");
        environment
            .Setup(mock => mock.DirectoriesInSearchPath)
            .Returns(new List<string> { "C:\\Agents", "D:\\Agents", "~\\bin" });

        environment
            .Setup(mock => mock.GetListOfFiles("C:\\Agents"))
            .Returns(new List<string>
            {
                "freshli-agent-java",
                "freshli-agent-java.bat",
                "freshli-agent-dotnet",
                "freshli-agent-dotnet.com",
                "other-one"
            });

        environment
            .Setup(mock => mock.GetListOfFiles("D:\\Agents"))
            .Returns(new List<string>
            {
                "freshli-agent-java",
                "freshli-agent-java.bat",
                "freshli-agent-ruby",
                "freshli-agent-ruby.exe",
                "ALLCAPS.BAT",
                "other-two"
            });

        environment
            .Setup(mock => mock.GetListOfFiles("C:\\Users\\user\\bin"))
            .Returns(new List<string> { "utility.bat" });

        environment
            .Setup(mock => mock.WindowsExecutableExtensions)
            .Returns(new List<string>
            {
                "exe",
                "bat",
                "com"
            });

        var expectedExecutables = new List<string>
        {
            "C:\\Agents\\freshli-agent-dotnet.com",
            "C:\\Agents\\freshli-agent-java.bat",
            "C:\\Users\\user\\bin\\utility.bat",
            "D:\\Agents\\ALLCAPS.BAT",
            "D:\\Agents\\freshli-agent-ruby.exe"
        };

        var finder = new ExecutableFinder(environment.Object);

        var actualExecutables = finder.GetExecutables();

        Assert.Equal(expectedExecutables, actualExecutables);
    }
}
