using System;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Test.Commands;
using Corgibytes.Freshli.Cli.Test.Common;
using PackageUrl;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.DependencyManagers;

[UnitTest]
public class AgentsRepositoryTest : FreshliTest
{
    private readonly AgentsRepository _repository;

    public AgentsRepositoryTest(ITestOutputHelper output) : base(output) =>
        _repository = new AgentsRepository(new MockAgentsDetector(), new MockAgentManager());

    [Fact]
    public void It_is_able_to_fetch_a_release_date_from_a_list()
    {
        var givenPackageUrl = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0");

        Assert.Equal(new DateTimeOffset(1990, 1, 29, 0, 0, 0, TimeSpan.Zero),
            _repository.GetReleaseDate(givenPackageUrl));
    }

    [Fact]
    public void It_is_able_to_fetch_the_latest_release_date()
    {
        var givenPackageUrl = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0");

        Assert.Equivalent(new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.3.0"),
            _repository.GetLatestVersion(givenPackageUrl));
    }

    [Theory]
    [InlineData("pkg:this.cant.be.real/org.corgibytes.random_package/randompackage@1.1.0",
        "None of the agents returned results for this package url")]
    [InlineData("pkg:ruby/org.corgibytes.no_release_date/no_release_date@1.1.0",
        "The returned list did not contain a release date for this package url")]
    public void It_errors_when_it_can_not_find_the_release_date(string packageUrl, string expectedErrorMessage)
    {
        var givenPackageUrl = new PackageURL(packageUrl);

        var caughtException =
            Assert.Throws<ReleaseDateNotFoundException>(() => _repository.GetReleaseDate(givenPackageUrl));

        Assert.Equal(expectedErrorMessage, caughtException.Message);
    }

    [Theory]
    [InlineData("pkg:this.cant.be.real/org.corgibytes.random_package/randompackage@1.1.0",
        "Latest version could not be found in list for this package url")]
    public void It_errors_when_it_can_not_find_the_latest_version(string packageUrl, string expectedErrorMessage)
    {
        var givenPackageUrl = new PackageURL(packageUrl);

        var caughtException =
            Assert.Throws<LatestVersionNotFoundException>(() => _repository.GetLatestVersion(givenPackageUrl));

        Assert.Equal(expectedErrorMessage, caughtException.Message);
    }
}
