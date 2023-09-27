using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Extensions;
using Microsoft.Extensions.Logging;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class PackageLibYearCalculator : IPackageLibYearCalculator
{
    private readonly ILogger<PackageLibYearCalculator>? _logger;
    private readonly IAgentReader _agentReader;
    private readonly PackageURL _packageUrl;
    private readonly DateTimeOffset _asOfDateTime;
    private List<Package>? _releaseHistory;

    public PackageLibYearCalculator(IAgentReader agentReader, PackageURL packageUrl, DateTimeOffset asOfDateTime, ILogger<PackageLibYearCalculator>? logger = null)
    {
        _logger = logger;
        _agentReader = agentReader;
        _packageUrl = packageUrl;
        _asOfDateTime = asOfDateTime;
    }

    [MemberNotNull(nameof(_releaseHistory))]
    private async Task EnsureReleaseHistory()
    {
        if (_releaseHistory != null)
        {
            return;
        }

        _releaseHistory = new List<Package>();
        await foreach (var package in _agentReader.RetrieveReleaseHistory(_packageUrl))
        {
            _releaseHistory.Add(package);
        }
    }

    public async ValueTask<PackageLibYear?> ComputeLibYear()
    {
        await EnsureReleaseHistory();

        var latestVersionPackageUrl = await GetLatestVersion();
        if (latestVersionPackageUrl == null)
        {
            return null;
        }

        var releaseDateCurrentVersion = await GetReleaseDate(_packageUrl);
        if (releaseDateCurrentVersion == null)
        {
            return null;
        }

        var releaseDateLatestVersion = await GetReleaseDate(latestVersionPackageUrl);
        if (releaseDateLatestVersion == null)
        {
            return null;
        }

        return new PackageLibYear(
            releaseDateCurrentVersion.Value,
            _packageUrl,
            releaseDateLatestVersion.Value,
            latestVersionPackageUrl,
            CalculateLibYear(releaseDateCurrentVersion.Value, releaseDateLatestVersion.Value),
            _asOfDateTime
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

    private async Task<DateTimeOffset?> GetReleaseDate(PackageURL packageUrl)
    {
        await EnsureReleaseHistory();

        // The problem is if the version of the packageUrl input parameter is no longer available, then the
        // exception will be thrown even though a compatible version might be available
        foreach (var package in _releaseHistory.Where(package => package.PackageUrl.PackageUrlEquals(packageUrl)))
        {
            return package.ReleasedAt;
        }

        await LogGetReleaseDateFailure(packageUrl);

        return null;
    }

    private async Task LogGetReleaseDateFailure(PackageURL packageUrl)
    {
        if (_logger == null)
        {
            return;
        }

        // only take the overhead hit for calculating this string if the
        // application is being debugged (i.e. trace enabled).
        if (!_logger.IsEnabled(LogLevel.Warning))
        {
            return;
        }

        await EnsureReleaseHistory();

        var versions = string.Join(", ", _releaseHistory.Select(release =>
            release.ReleasedAt.ToString("yyyy-MM-dd") + "@" + release.PackageUrl.Version));
        _logger.LogWarning(
            "[Agent: {Agent}] Failed to find Release date for {PackageUrl} from history = {Versions}",
            _agentReader.Name, packageUrl, versions);
    }

    private async Task<PackageURL?> GetLatestVersion()
    {
        await EnsureReleaseHistory();

        IEnumerable<Package> history = _releaseHistory.ToList();
        var latestPackage = history
            .OrderByDescending(package => package.ReleasedAt)
            .FirstOrDefault(package => package.ReleasedAt < _asOfDateTime);

        if (latestPackage != null)
        {
            return latestPackage.PackageUrl;
        }

        LogGetLatestVersionFailure();

        return null;
    }

    private void LogGetLatestVersionFailure()
    {
        _logger?.LogDebug("[Agent: {Agent}] Failed to find latest version for package = {PackageUrl} asOfDate = {AsOfDate}",
            _agentReader.Name, _packageUrl, _asOfDateTime);
    }
}
