using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Test.Common;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class LibYearTest : FreshliTest
{
    public LibYearTest(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [MethodData(nameof(ExpectedLibYears))]
    public void Validate_expected_libyears(DateTimeOffset releaseDateCurrentVersion,
        DateTimeOffset releaseDateLatestVersion, double expectedLibYear,
        int precision) => Assert.Equal(expectedLibYear,
        LibYear.GivenReleaseDates(releaseDateCurrentVersion, releaseDateLatestVersion).AsDecimalNumber(precision));

    private static TheoryData<DateTimeOffset, DateTimeOffset, double, int> ExpectedLibYears() =>
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
