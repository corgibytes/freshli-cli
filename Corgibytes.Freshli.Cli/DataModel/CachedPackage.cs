using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DataModel;

public class CachedPackage
{
    public CachedPackage(PackageURL packageUrl, DateTimeOffset releasedAt)
    {
        PackageUrl = packageUrl;
        ReleasedAt = releasedAt;
    }

    public PackageURL PackageUrl { get; }
    public DateTimeOffset ReleasedAt { get; }
}
