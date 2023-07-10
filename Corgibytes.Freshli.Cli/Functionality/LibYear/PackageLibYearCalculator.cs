using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Services;
using NLog;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class PackageLibYearCalculator : IPackageLibYearCalculator
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public async ValueTask<PackageLibYear> ComputeLibYear(IAgentReader agentReader, PackageURL packageUrl, DateTimeOffset asOfDateTime)
    {
        var releaseHistory = new List<Package>();
        await foreach (var package in agentReader.RetrieveReleaseHistory(packageUrl))
        {
            releaseHistory.Add(package);
        }

        var latestVersionPackageUrl = GetLatestVersion(releaseHistory, asOfDateTime);
        var releaseDateCurrentVersion = GetReleaseDate(releaseHistory, packageUrl);
        var releaseDateLatestVersion = GetReleaseDate(releaseHistory, latestVersionPackageUrl);

        return new PackageLibYear(
            releaseDateCurrentVersion,
            packageUrl,
            releaseDateLatestVersion,
            latestVersionPackageUrl,
            CalculateLibYear(releaseDateCurrentVersion, releaseDateLatestVersion),
            asOfDateTime
        );
    }

    public static double CalculateLibYear(DateTimeOffset releaseDateCurrentVersion,
        DateTimeOffset releaseDateLatestVersion, int precision = 2)
    {
        // Note: This next bit is under discussion: https://github.com/corgibytes/freshli-cli/issues/114

        // .Duration() will always return an absolute value.
        // So even if the latest version was released before the current version you'll end up with a positive number.
        var timeSpan = releaseDateLatestVersion.Subtract(releaseDateCurrentVersion).Duration();

        var numberOfLeapYearsBetween =
            LeapYears.NumberOfLeapYearsBetween(releaseDateCurrentVersion.Year, releaseDateLatestVersion.Year);

        if (releaseDateCurrentVersion.Year == releaseDateLatestVersion.Year || numberOfLeapYearsBetween == 0)
        {
            return Math.Round((double)timeSpan.Days / 365, precision);
        }

        // An example:
        // Given release date current version 1990, and release date latest version 2021.
        // There's 31 years between the two:
        // 31 * 365 = 11315 days if no leap years present
        // ((31 - 8) * 365) + (8 * 364) = 11307 days with leap years
        // Average of 364.75 days per year when counting leap years.

        var totalYears = Math.Abs(releaseDateLatestVersion.Year - releaseDateCurrentVersion.Year);
        var averageNumberOfDaysPerYear = (double)
            ((totalYears - numberOfLeapYearsBetween) * 365 + numberOfLeapYearsBetween * 364) / totalYears;

        return Math.Round(timeSpan.Days / averageNumberOfDaysPerYear, precision);
    }

    private static DateTimeOffset GetReleaseDate(IEnumerable<Package> releaseHistory, PackageURL packageUrl)
    {
        foreach (var package in releaseHistory.Where(package => package.PackageUrl.PackageUrlEquals(packageUrl)))
        {
            return package.ReleasedAt;
        }
        // only take the overhead hit for calculating this string if the
        // application is being debugged (i.e. trace enabled).
        if (s_logger.IsTraceEnabled)
        {
            var versions = string.Join(", ", releaseHistory.Select(release =>
                release.ReleasedAt.ToString("yyyy-MM-dd") + "@" + release.PackageUrl.Version));
            s_logger.Trace($"Failed to find Release date for {packageUrl} from history = {versions}");
        }

        throw ReleaseDateNotFoundException.BecauseReturnedListDidNotContainReleaseDate(packageUrl.ToString());
    }

    private static PackageURL GetLatestVersion(IEnumerable<Package> releaseHistory, DateTimeOffset asOfDate)
    {
        var latestPackage = releaseHistory
            .OrderByDescending(package => package.ReleasedAt)
            .FirstOrDefault(package => package.ReleasedAt < asOfDate);

        if (latestPackage != null)
        {
            return latestPackage.PackageUrl;
        }
        var packageUrl = releaseHistory.First().PackageUrl;
        s_logger.Debug("Failed to find latest version for package = {PackageUrl} asOfDate = {AsOfDate}",
            packageUrl, asOfDate);

        throw LatestVersionNotFoundException.BecauseLatestCouldNotBeFoundInList(packageUrl.ToString(), asOfDate);
    }
}
