using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class LibYearTest : FreshliTest
{
    public LibYearTest(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [ClassData(typeof(LibYearTestDataGenerator))]
    public void Validate_expected_libyears(DateTime releaseDateCurrentVersion, DateTime releaseDateLatestVersion, double expectedLibYear, int precision)
    {
        Assert.Equal(expectedLibYear, LibYear.GivenReleaseDates(releaseDateCurrentVersion, releaseDateLatestVersion).AsDecimalNumber(precision));
    }
}
