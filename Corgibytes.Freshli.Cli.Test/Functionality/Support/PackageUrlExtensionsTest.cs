using Corgibytes.Freshli.Cli.Functionality.Extensions;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Support;

public class PackageUrlExtensionsTest
{

    [Theory]
    [InlineData("pkg:nuget/Corgibytes.Freshli.Lib@0.5.0", "nuget", "Corgibytes.Freshli.Lib", "0.5.0")]
    [InlineData("pkg:nuget/Jil@3.0.0-alpha2", "nuget", "Jil", "3.0.0-alpha2")]
    [InlineData("pkg:nuget/Microsoft.Extensions.Caching.Abstractions@7.0.0-preview.3.22175.4", "nuget", "Microsoft.Extensions.Caching.Abstractions", "7.0.0-preview.3.22175.4")]
    [InlineData("pkg:nuget/Microsoft.Extensions.Caching.Abstractions@8.0.0-preview.5.23280.8", "nuget", "Microsoft.Extensions.Caching.Abstractions", "8.0.0-preview.5.23280.8")]
    [InlineData("pkg:nuget/Microsoft.Extensions.Configuration@6.0.0-rc.2.21480.5", "nuget", "Microsoft.Extensions.Configuration", "6.0.0-rc.2.21480.5")]
    [InlineData("pkg:nuget/Corgibytes/Freshli.Lib@0.5.0", "nuget", "Corgibytes.Freshli.Lib", "0.5.0", false)]
    [InlineData("pkg:nuget/Corgibytes/Freshli.Lib@0.5.0", "nuget", "Corgibytes/Freshli.Lib", "0.5.0.1", false)]
    [InlineData("pkg:nuget/PropertyChanged.Fody@1.41.0.0", "nuget", "PropertyChanged.Fody", "1.41.0")]
    public void IsPackageUrlEqual(string purl,
        string type, string name, string version, bool expectEqual = true)
    {
        var packageUrl = new PackageURL(purl);
        Assert.Equal(expectEqual, packageUrl.PackageUrlEquals(
            new PackageURL(type, null, name, version, null, null)
        ));
    }

    [Fact]
    public void IsPackageUrlNull()
    {
        var packageUrl = new PackageURL("pkg:nuget/Corgibytes/Freshli.Lib@0.5.0");
        Assert.False(packageUrl.PackageUrlEquals(null));
    }
}
