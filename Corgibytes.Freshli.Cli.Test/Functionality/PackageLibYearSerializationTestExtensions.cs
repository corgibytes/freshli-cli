using System;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public static class PackageLibYearSerializationTestExtensions
{
    // ReSharper disable once UnusedParameter.Global
    public static PackageLibYear BuildPackageLibYear(this SerializationTest _) =>
        new(
            new DateTimeOffset(2021, 11, 22, 11, 22, 33, 44, TimeSpan.Zero),
            new PackageURL("pkg:maven/org.apache.xmlgraphics/batik-anim@1.9.1?repository_url=repo.spring.io%2Frelease"),
            new DateTimeOffset(2021, 11, 23, 11, 22, 33, 44, TimeSpan.Zero),
            new PackageURL("pkg:maven/org.apache.xmlgraphics/batik-anim@1.10?repository_url=repo.spring.io%2Frelease"),
            12.5,
            new PackageURL("pkg:maven/org.apache.xmlgraphics/batik-anim@1.11?repository_url=repo.spring.io%2Frelease"),
            new DateTimeOffset(2021, 11, 24, 11, 22, 33, 44, TimeSpan.Zero),
            "sample message"
        );

    // ReSharper disable once UnusedParameter.Global
    public static void AssertPackageLibYearEqual(this SerializationTest _, PackageLibYear incoming,
        PackageLibYear outgoing)
    {
        Assert.Equal(incoming.CurrentVersion!.ToString(), outgoing.CurrentVersion!.ToString());
        Assert.Equal(incoming.ExceptionMessage, outgoing.ExceptionMessage);
        Assert.Equal(incoming.LatestVersion!.ToString(), outgoing.LatestVersion!.ToString());
        Assert.Equal(incoming.LibYear, outgoing.LibYear);
        Assert.Equal(incoming.PackageUrl!.ToString(), outgoing.PackageUrl!.ToString());
        Assert.Equal(incoming.ReleaseDateCurrentVersion, outgoing.ReleaseDateCurrentVersion);
        Assert.Equal(incoming.ReleaseDateLatestVersion, outgoing.ReleaseDateLatestVersion);
        Assert.Equal(incoming.AsOfDateTime, outgoing.AsOfDateTime);
    }
}
