using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

public class PackageLibYearCalculatorTest
{
    [Fact]
    public void VerifyItCanCalculateTheLibYear()
    {
        var packageName = "pkg:maven/org.apache.maven/apache-maven";
        var currentlyInstalledPackageUrl = new PackageURL(packageName + "@1.3.4");

        var givenReleaseHistory = new List<Package>
        {
            new(
                // This version was released _after_ the asOfDateTime so it should not count
                new PackageURL(packageName + "@2.3.1"),
                new DateTimeOffset(2022, 1, 29, 12, 30, 45, TimeSpan.Zero)
            ),
            new(
                new PackageURL(packageName + "@2.2.1"),
                new DateTimeOffset(2021, 1, 29, 12, 30, 45, TimeSpan.Zero)
            ),
            new(
                new PackageURL(packageName + "@2.0.0"),
                new DateTimeOffset(2020, 2, 29, 12, 30, 45, TimeSpan.Zero)
            ),
            new(
                new PackageURL(packageName + "@1.3.5"),
                new DateTimeOffset(2019, 6, 1, 12, 30, 45, TimeSpan.Zero)
            ),
            new(
                currentlyInstalledPackageUrl,
                new DateTimeOffset(2019, 3, 30, 14, 41, 55, TimeSpan.Zero)
            ),
        };

        var agentReader = new Mock<IAgentReader>();
        agentReader.Setup(mock => mock.RetrieveReleaseHistory(currentlyInstalledPackageUrl)).Returns(givenReleaseHistory);

        var calculator = new PackageLibYearCalculator();
        var asOfDateTime = new DateTimeOffset(2021, 12, 31, 12, 30, 45, TimeSpan.Zero);

        var expectedPackageLibYear = new PackageLibYear(
            new DateTimeOffset(2019, 3, 30, 14, 41, 55, TimeSpan.Zero),
            currentlyInstalledPackageUrl,
            new DateTimeOffset(2021, 1, 29, 12, 30, 45, TimeSpan.Zero),
            new PackageURL(packageName + "@2.2.1"),
            1.84,
            asOfDateTime
        );

        var actualPackageLibYear = calculator.ComputeLibYear(agentReader.Object, currentlyInstalledPackageUrl, asOfDateTime);

        Assert.Equivalent(expectedPackageLibYear.CurrentVersion!, actualPackageLibYear.CurrentVersion!);
        Assert.Equivalent(expectedPackageLibYear.LatestVersion!, actualPackageLibYear.LatestVersion!);
        Assert.Equal(expectedPackageLibYear.ReleaseDateCurrentVersion, actualPackageLibYear.ReleaseDateCurrentVersion);
        Assert.Equal(expectedPackageLibYear.ReleaseDateLatestVersion, actualPackageLibYear.ReleaseDateLatestVersion);
        Assert.Equal(expectedPackageLibYear.LibYear, actualPackageLibYear.LibYear);
    }

    [Theory]
    [MemberData(nameof(ExpectedLibYears))]
    public void VerifyExpectedLibYears(
        DateTimeOffset releaseDateCurrentVersion, DateTimeOffset releaseDateLatestVersion, double expectedLibYear, int precision
    ) =>
        Assert.Equal(expectedLibYear, PackageLibYearCalculator.CalculateLibYear(releaseDateCurrentVersion, releaseDateLatestVersion, precision));

    public static TheoryData<DateTimeOffset, DateTimeOffset, double, int> ExpectedLibYears() =>
        new()
        {
            {
                // Release date current version, release date latest version, expected libyear
                // Case: new version released in 2021, current version from 2019
                new DateTimeOffset(2019, 1, 3, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2021, 8, 25, 0, 0, 0, TimeSpan.Zero), 2.65, 2
            },
            {
                // Case: new version released in 2021, current version from 2019
                // Higher precision
                new DateTimeOffset(2019, 1, 3, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2021, 8, 25, 0, 0, 0, TimeSpan.Zero), 2.64746, 5
            },
            {
                // Case: new version released in 2020, current version from 2021.
                // Example: Symfony 4 is maintained, and gets security updates til 2024. Latest version is Symfony 6. Symfony 6 last release was 2021, Symfony 4 had a security update in 2022.
                new DateTimeOffset(2022, 6, 14, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2021, 9, 21, 0, 0, 0, TimeSpan.Zero), 0.73, 2
            },
            {
                // Case: new version released in 2021, current version from 1990
                // Higher precision, and we have to deal with leap years
                new DateTimeOffset(1990, 1, 3, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2021, 1, 3, 0, 0, 0, TimeSpan.Zero), 31.04387, 5
            },
            {
                // Case: new version released in 2004, current version from 2004
                // This is a leap year, see if it still ends up as 1
                new DateTimeOffset(2004, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2004, 12, 31, 0, 0, 0, TimeSpan.Zero), 1.00, 2
            }
        };
}

