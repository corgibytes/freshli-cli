using Corgibytes.Freshli.Cli.Extensions;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Extensions;

public class PackageUrlExtensionsTest
{
    [Fact]
    public void IsPackageUrlEqual()
    {
        var packageUrl = new PackageURL("pkg:nuget/Corgibytes.Freshli.Lib@0.5.0");
        Assert.True(packageUrl.PackageUrlEquals(
            new PackageURL("nuget", null, "Corgibytes.Freshli.Lib",
                "0.5.0", null, null)
        ));
    }

    [Fact]
    public void IsPackageUrlNotEqual()
    {
        var packageUrl = new PackageURL("pkg:nuget/Corgibytes/Freshli.Lib@0.5.0");
        Assert.False(packageUrl.PackageUrlEquals(
            new PackageURL("nuget", null, "Corgibytes.Freshli.Lib",
                "0.5.0", null, null)
        ));
    }

    [Fact]
    public void IsPackageUrlNull()
    {
        var packageUrl = new PackageURL("pkg:nuget/Corgibytes/Freshli.Lib@0.5.0");
        Assert.False(packageUrl.PackageUrlEquals(null));
    }
}
