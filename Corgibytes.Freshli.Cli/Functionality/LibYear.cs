using System;

namespace Corgibytes.Freshli.Cli.Functionality;

public class LibYear
{
    private readonly DateTimeOffset _releaseDateCurrentVersion;
    private readonly DateTimeOffset _releaseDateLatestVersion;

    private LibYear(DateTimeOffset releaseDateCurrentVersion, DateTimeOffset releaseDateLatestVersion)
    {
        _releaseDateCurrentVersion = releaseDateCurrentVersion;
        _releaseDateLatestVersion = releaseDateLatestVersion;
    }

    private TimeSpan TimeSpan { get; init; }

    public static LibYear GivenReleaseDates(DateTimeOffset releaseDateCurrentVersion,
        DateTimeOffset releaseDateLatestVersion)
    {
        // .Duration() will always return an absolute value.
        // So even if the latest version was released before the current version you'll end up with a positive number.
        var libYear = new LibYear(releaseDateCurrentVersion, releaseDateLatestVersion)
        {
            TimeSpan = releaseDateLatestVersion.Subtract(releaseDateCurrentVersion).Duration()
        };

        return libYear;
    }

    public double AsDecimalNumber(int precision = 2)
    {
        var numberOfLeapYearsBetween =
            LeapYears.NumberOfLeapYearsBetween(_releaseDateCurrentVersion.Year, _releaseDateLatestVersion.Year);

        if (_releaseDateCurrentVersion.Year == _releaseDateLatestVersion.Year || numberOfLeapYearsBetween == 0)
        {
            return Math.Round((double)TimeSpan.Days / 365, precision);
        }

        // An example:
        // Given release date current version 1990, and release date latest version 2021.
        // There's 31 years between the two:
        // 31 * 365 = 11315 days if no leap years present
        // ((31 - 8) * 365) + (8 * 364) = 11307 days with leap years
        // Average of 364.75 days per year when counting leap years.

        var totalYears = Math.Abs(_releaseDateLatestVersion.Year - _releaseDateCurrentVersion.Year);
        var averageNumberOfDaysPerYear = (double)
            ((totalYears - numberOfLeapYearsBetween) * 365 + numberOfLeapYearsBetween * 364) / totalYears;

        return Math.Round(TimeSpan.Days / averageNumberOfDaysPerYear, precision);
    }
}
