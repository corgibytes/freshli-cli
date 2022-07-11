using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public class PackageLibYear
{
    public readonly PackageURL CurrentVersion;
    public readonly string ExceptionMessage;
    public readonly PackageURL LatestVersion;
    public readonly double LibYear;
    public readonly PackageURL PackageUrl;
    public DateTimeOffset ReleaseDateCurrentVersion;
    public DateTimeOffset ReleaseDateLatestVersion;

    public PackageLibYear(DateTimeOffset releaseDateCurrentVersion, PackageURL currentVersion,
        DateTimeOffset releaseDateLatestVersion, PackageURL latestVersion, double libYear)
    {
        ReleaseDateCurrentVersion = releaseDateCurrentVersion;
        CurrentVersion = currentVersion;
        ReleaseDateLatestVersion = releaseDateLatestVersion;
        LatestVersion = latestVersion;
        LibYear = libYear;
    }

    public PackageLibYear(PackageURL packageUrl, string exceptionMessage)
    {
        PackageUrl = packageUrl;
        ExceptionMessage = exceptionMessage;
    }
}
