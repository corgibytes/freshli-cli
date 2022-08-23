using System;
using Newtonsoft.Json;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality;

public class PackageLibYear
{
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

    [JsonConstructor]
    public PackageLibYear(DateTimeOffset releaseDateCurrentVersion, PackageURL currentVersion,
        DateTimeOffset releaseDateLatestVersion, PackageURL latestVersion, double libYear,
        PackageURL packageUrl, string exceptionMessage)
    {
        ReleaseDateCurrentVersion = releaseDateCurrentVersion;
        CurrentVersion = currentVersion;
        ReleaseDateLatestVersion = releaseDateLatestVersion;
        LatestVersion = latestVersion;
        LibYear = libYear;
        PackageUrl = packageUrl;
        ExceptionMessage = exceptionMessage;
    }

    public PackageURL? CurrentVersion { get; }
    public string? ExceptionMessage { get; }
    public PackageURL? LatestVersion { get; }
    public double LibYear { get; }
    public PackageURL? PackageUrl { get; }
    public DateTimeOffset ReleaseDateCurrentVersion { get; }
    public DateTimeOffset ReleaseDateLatestVersion { get; }
}
