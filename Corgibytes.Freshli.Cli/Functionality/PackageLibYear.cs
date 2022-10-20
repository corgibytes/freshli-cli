using System;
using Newtonsoft.Json;
using PackageUrl;

// Currently the public fields aren't being used but once we get the API call to submit the results to we'll need these to be public
// Related: https://github.com/corgibytes/freshli-cli/issues/235
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.Functionality;

public class PackageLibYear
{
    // todo: remove this constructor - it's being kept to support existing tests
    public PackageLibYear(DateTimeOffset releaseDateCurrentVersion, PackageURL currentVersion,
        DateTimeOffset releaseDateLatestVersion, PackageURL latestVersion, double libYear)
    {
        AsOfDateTime = default;
        ReleaseDateCurrentVersion = releaseDateCurrentVersion;
        CurrentVersion = currentVersion;
        ReleaseDateLatestVersion = releaseDateLatestVersion;
        LatestVersion = latestVersion;
        LibYear = libYear;
    }


    public PackageLibYear(DateTimeOffset releaseDateCurrentVersion, PackageURL currentVersion,
        DateTimeOffset releaseDateLatestVersion, PackageURL latestVersion, double libYear,
        DateTimeOffset asOfDateTime)
    {
        AsOfDateTime = asOfDateTime;
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
        PackageURL packageUrl, DateTimeOffset asOfDateTime, string exceptionMessage)
    {
        ReleaseDateCurrentVersion = releaseDateCurrentVersion;
        CurrentVersion = currentVersion;
        ReleaseDateLatestVersion = releaseDateLatestVersion;
        LatestVersion = latestVersion;
        LibYear = libYear;
        PackageUrl = packageUrl;
        AsOfDateTime = asOfDateTime;
        ExceptionMessage = exceptionMessage;
    }

    public PackageURL? CurrentVersion { get; }
    public string? ExceptionMessage { get; }
    public PackageURL? LatestVersion { get; }
    public double LibYear { get; }
    public PackageURL? PackageUrl { get; }
    public DateTimeOffset ReleaseDateCurrentVersion { get; }
    public DateTimeOffset ReleaseDateLatestVersion { get; }
    public DateTimeOffset AsOfDateTime { get; }
}
