using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Package
{
    public Package(PackageURL packageUrl, DateTimeOffset releasedAt)
    {
        PackageUrl = packageUrl;
        ReleasedAt = releasedAt;
    }

    public PackageURL PackageUrl { get; }
    public DateTimeOffset ReleasedAt { get; }
}
