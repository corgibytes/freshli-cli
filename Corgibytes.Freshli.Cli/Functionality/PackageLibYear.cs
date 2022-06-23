using System;

namespace Corgibytes.Freshli.Cli.Functionality;

public class PackageLibYear
{
    public string PackageName;
    public DateTimeOffset ReleaseDateCurrentVersion;
    public string CurrentVersion;
    public DateTimeOffset ReleaseDateLatestVersion;
    public string LatestVersion;
    public double LibYear;

    public PackageLibYear(string packageName, DateTimeOffset releaseDateCurrentVersion, string currentVersion, DateTimeOffset releaseDateLatestVersion, string latestVersion, double libYear)
    {
        PackageName = packageName;
        ReleaseDateCurrentVersion = releaseDateCurrentVersion;
        CurrentVersion = currentVersion;
        ReleaseDateLatestVersion = releaseDateLatestVersion;
        LatestVersion = latestVersion;
        LibYear = libYear;
    }
}

