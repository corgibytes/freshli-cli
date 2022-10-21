using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Package : IComparable<Package>
{
    public Package(PackageURL packageUrl, DateTimeOffset releasedAt)
    {
        PackageUrl = packageUrl;
        ReleasedAt = releasedAt;
    }

    public PackageURL PackageUrl { get; }
    public DateTimeOffset ReleasedAt { get; }

    public int CompareTo(Package? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other == null)
        {
            return 1;
        }

        var result = string.Compare(PackageUrl.ToString(), other.PackageUrl.ToString(), StringComparison.Ordinal);
        if (result == 0)
        {
            result = ReleasedAt.CompareTo(other.ReleasedAt);
        }

        return result;
    }
}
