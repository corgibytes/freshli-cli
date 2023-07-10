using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Corgibytes.Freshli.Cli.Services;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.LibYear;

public class PackageLibYearCalculatorTest
{
    [Fact(Timeout = 500)]
    public async Task VerifyItCanCalculateTheLibYear()
    {
        const string packageName = "pkg:maven/org.apache.maven/apache-maven";
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
            )
        };

        var agentReader = new Mock<IAgentReader>();
        agentReader.Setup(mock => mock.RetrieveReleaseHistory(currentlyInstalledPackageUrl))
            .Returns(givenReleaseHistory.ToAsyncEnumerable());

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

        var actualPackageLibYear =
            await calculator.ComputeLibYear(agentReader.Object, currentlyInstalledPackageUrl, asOfDateTime);

        Assert.Equivalent(expectedPackageLibYear.CurrentVersion!, actualPackageLibYear.CurrentVersion!);
        Assert.Equivalent(expectedPackageLibYear.LatestVersion!, actualPackageLibYear.LatestVersion!);
        Assert.Equal(expectedPackageLibYear.ReleaseDateCurrentVersion, actualPackageLibYear.ReleaseDateCurrentVersion);
        Assert.Equal(expectedPackageLibYear.ReleaseDateLatestVersion, actualPackageLibYear.ReleaseDateLatestVersion);
        Assert.Equal(expectedPackageLibYear.LibYear, actualPackageLibYear.LibYear);
    }

    [Theory]
    [MemberData(nameof(ExpectedPackageLibYears))]
    public void VerifyNLogHasReleaseDate(PackageLibYear expectedPackageLibYear)
    {
        var packageUrl = new PackageURL("pkg:nuget/NLog@4.7.7");
        var releaseHistory = LoadReleaseHistory("nuget.NLog-ReleaseHistory.json");

        var computedLibYear =
            PackageLibYearCalculator.ComputeLibYear(packageUrl, expectedPackageLibYear.AsOfDateTime, releaseHistory);
        Assert.NotNull(computedLibYear);
        Assert.Equivalent(expectedPackageLibYear.CurrentVersion!, computedLibYear.CurrentVersion!);
        Assert.Equivalent(expectedPackageLibYear.LatestVersion!, computedLibYear.LatestVersion!);
        Assert.Equal(expectedPackageLibYear.ReleaseDateCurrentVersion, computedLibYear.ReleaseDateCurrentVersion);
        Assert.Equal(expectedPackageLibYear.ReleaseDateLatestVersion, computedLibYear.ReleaseDateLatestVersion);
        Assert.Equal(expectedPackageLibYear.LibYear, computedLibYear.LibYear);
    }

    [Fact]
    public void VerifyNLogNoReleaseDate()
    {
        var packageUrl = new PackageURL("pkg:nuget/NLog@4.7.7");
        var releaseHistory = LoadReleaseHistory("nuget.NLog-ReleaseHistory.json");
        var asOfDate = DateTimeOffset.Parse("1900-01-01T00:00:00.000Z").AddDays(-7);
        var computedLibYear = PackageLibYearCalculator.ComputeLibYear(packageUrl, asOfDate, releaseHistory);
        Assert.Null(computedLibYear);
    }

    private static List<Package> LoadReleaseHistory(string releaseHistoryDataFile)
    {
        var releaseHistoryFilename = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Fixtures", "Functionality", "LibYear", releaseHistoryDataFile);

        var readAllText = File.ReadAllText(releaseHistoryFilename);

        var releaseHistory = new List<Package>();
        var json = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(readAllText);
        foreach (var historyJson in json)
        {
            var purl = new PackageURL("pkg:nuget/NLog@" + historyJson["version"]);
            releaseHistory.Add(new Package(
                purl, DateTimeOffset.Parse(historyJson["released_at"]))
            );
        }

        return releaseHistory;
    }

    [Theory]
    [MemberData(nameof(ExpectedLibYears))]
    public void VerifyExpectedLibYears(
        DateTimeOffset releaseDateCurrentVersion, DateTimeOffset releaseDateLatestVersion, double expectedLibYear,
        int precision
    ) =>
        Assert.Equal(expectedLibYear,
            PackageLibYearCalculator.CalculateLibYear(releaseDateCurrentVersion, releaseDateLatestVersion, precision));

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

    public static TheoryData<PackageLibYear> ExpectedPackageLibYears() =>
        new()
        {
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2023-05-30 17:54:25.0300000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.2.0"), 2.35,
                DateTimeOffset.Parse("2023-06-08 21:56:45.7111888+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2023-05-30 17:54:25.0300000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.2.0"), 2.35,
                DateTimeOffset.Parse("2023-06-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2023-04-29 10:29:37.5730000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.1.4"), 2.27,
                DateTimeOffset.Parse("2023-05-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2023-03-28 19:35:24.3500000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.1.3"), 2.18,
                DateTimeOffset.Parse("2023-04-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2023-02-17 17:20:09.9870000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.1.2"), 2.07,
                DateTimeOffset.Parse("2023-03-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-12-29 23:17:37.1400000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.1.1"), 1.94,
                DateTimeOffset.Parse("2023-02-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-12-29 23:17:37.1400000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.1.1"), 1.94,
                DateTimeOffset.Parse("2023-01-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-11-27 10:01:10.3100000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.1.0"), 1.85,
                DateTimeOffset.Parse("2022-12-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-10-25 22:42:21.6670000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.0.5"), 1.76,
                DateTimeOffset.Parse("2022-11-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-09-01 21:42:44.9970000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.0.4"), 1.61,
                DateTimeOffset.Parse("2022-10-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-08-31 20:28:55.7170000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.0.3"), 1.61,
                DateTimeOffset.Parse("2022-09-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-06-12 13:52:12.4770000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.0.1"), 1.39,
                DateTimeOffset.Parse("2022-08-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-06-12 13:52:12.4770000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.0.1"), 1.39,
                DateTimeOffset.Parse("2022-07-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-05-16 20:41:23.4530000+00:00"),
                new PackageURL("pkg:nuget/NLog@5.0.0"), 1.32,
                DateTimeOffset.Parse("2022-06-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-03-26 15:01:52.3100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.15"), 1.18,
                DateTimeOffset.Parse("2022-05-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-03-26 15:01:52.3100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.15"), 1.18,
                DateTimeOffset.Parse("2022-04-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2022-02-23 07:04:46.4770000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.14"), 1.09,
                DateTimeOffset.Parse("2022-03-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-12-05 22:01:03.9600000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.13"), 0.87,
                DateTimeOffset.Parse("2022-02-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-12-05 22:01:03.9600000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.13"), 0.87,
                DateTimeOffset.Parse("2022-01-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-10-24 18:41:38.1170000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.12"), 0.76,
                DateTimeOffset.Parse("2021-12-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-10-24 18:41:38.1170000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.12"), 0.76,
                DateTimeOffset.Parse("2021-11-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-08-18 22:22:53.2030000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.11"), 0.57,
                DateTimeOffset.Parse("2021-10-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-08-18 22:22:53.2030000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.11"), 0.57,
                DateTimeOffset.Parse("2021-09-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-05-14 12:44:55.4800000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.10"), 0.31,
                DateTimeOffset.Parse("2021-08-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-05-14 12:44:55.4800000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.10"), 0.31,
                DateTimeOffset.Parse("2021-07-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-05-14 12:44:55.4800000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.10"), 0.31,
                DateTimeOffset.Parse("2021-06-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-03-24 22:22:53.3330000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.9"), 0.17,
                DateTimeOffset.Parse("2021-05-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-03-24 22:22:53.3330000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.9"), 0.17,
                DateTimeOffset.Parse("2021-04-01 00:00:00+00:00")),
            new PackageLibYear(
                DateTimeOffset.Parse("2021-01-20 23:25:04.4100000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.7"),
                DateTimeOffset.Parse("2021-02-25 23:59:55.4530000+00:00"),
                new PackageURL("pkg:nuget/NLog@4.7.8"), 0.1,
                DateTimeOffset.Parse("2021-03-01 00:00:00+00:00")),
        };
}
