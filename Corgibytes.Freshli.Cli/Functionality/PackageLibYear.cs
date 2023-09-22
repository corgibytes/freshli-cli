using System;
using PackageUrl;

// Currently the public fields aren't being used but once we get the API call to submit the results to we'll need these to be public
// Related: https://github.com/corgibytes/freshli-cli/issues/235
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.Functionality;

public class PackageLibYear
{
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

    public PackageURL CurrentVersion { get; }
    public PackageURL LatestVersion { get; }
    public double LibYear { get; }
    public DateTimeOffset ReleaseDateCurrentVersion { get; }
    public DateTimeOffset ReleaseDateLatestVersion { get; }
    public DateTimeOffset AsOfDateTime { get; }

    public override string ToString()
    {
        return $"CurrentVersion = {CurrentVersion}, LatestVersion = {LatestVersion}, LibYear = {LibYear}, ReleaseDateCurrentVersion = {ReleaseDateCurrentVersion}, ReleaseDateLatestVersion = {ReleaseDateLatestVersion}, AsOfDateTime = {AsOfDateTime}";
    }
}
