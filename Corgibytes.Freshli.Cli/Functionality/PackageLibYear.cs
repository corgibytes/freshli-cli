using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public class PackageLibYear
{
    public PackageURL PackageUrl;
    public string ExceptionMessage;
    public DateTimeOffset ReleaseDateCurrentVersion;
    public PackageURL CurrentVersion;
    public DateTimeOffset ReleaseDateLatestVersion;
    public PackageURL LatestVersion;
    public double LibYear;

    public PackageLibYear(DateTimeOffset releaseDateCurrentVersion, PackageURL currentVersion, DateTimeOffset releaseDateLatestVersion, PackageURL latestVersion, double libYear)
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

