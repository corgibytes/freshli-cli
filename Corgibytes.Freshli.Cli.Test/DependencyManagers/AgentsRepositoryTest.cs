using System;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Test.Commands;
using Corgibytes.Freshli.Cli.Test.Common;
using PackageUrl;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.DependencyManagers;

public class AgentsRepositoryTest : FreshliTest
{
    private readonly AgentsRepository _repository;

    public AgentsRepositoryTest(ITestOutputHelper output) : base(output)
    {
        _repository = new(new MockAgentsDetector(), new MockAgentsReader());
    }

    [Fact]
    public void It_is_able_to_fetch_a_release_date_from_a_list()
    {
        var givenPackageUrl = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0");

        Assert.Equal(new(1990, 1, 29, 0, 0, 0, TimeSpan.Zero), _repository.GetReleaseDate(givenPackageUrl));
    }

    [Theory]
    [InlineData("pkg:this.cant.be.real/org.corgibytes.random_package/randompackage@1.1.0", "None of the agents returned results for this package url: pkg:this.cant.be.real/org.corgibytes.random_package/randompackage@1.1.0")]
    [InlineData("pkg:ruby/org.corgibytes.no_release_date/no_release_date@1.1.0", "The returned list did not contain a release date for this package url: pkg:ruby/org.corgibytes.no_release_date/no_release_date@1.1.0")]
    public void It_errors_when_it_cant_process_the_package_url(string packageUrl, string expectedErrorMessage)
    {
        var givenPackageUrl = new PackageURL(packageUrl);

        var caughtException = Assert.Throws<ReleaseDateNotFoundException>(() => _repository.GetReleaseDate(givenPackageUrl));

        Assert.Equal(expectedErrorMessage, caughtException.Message);
    }
}
