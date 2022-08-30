using System;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;
using Xunit;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test.DependencyManagers;

[IntegrationTest]
public class AgentsRepositoryIntegrationTest
{
    [Fact]
    public void GetReleaseDate()
    {
        var repository = new AgentsRepository(new AgentsDetector(new Environment()), new AgentManager());

        var actualReleaseDate =
            repository.GetReleaseDate(new PackageURL("pkg:maven/org.apache.maven/apache-maven@2.2.1"));

        var expectedReleaseDate = DateTimeOffset.Parse("2009-08-06T19:18:53Z");

        Assert.Equal(expectedReleaseDate, actualReleaseDate);
    }

    [Fact]
    public void GetLatestVersion()
    {
        var repository = new AgentsRepository(new AgentsDetector(new Environment()), new AgentManager());
        var latestVersion =
            repository.GetLatestVersion(new PackageURL("pkg:maven/org.apache.maven/apache-maven@2.2.1"));

        Assert.True(string.Compare(latestVersion.Version, "3.8.5", StringComparison.InvariantCulture) > 0,
            $"Expected {latestVersion.Version} to be greater than 3.8.5.");
    }

}
