
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Functionality
{
    public class LeapYearsTest: FreshliTest
    {
        public LeapYearsTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(1990, 2021, 8)]
        [InlineData(2021, 2021, 0)]
        // Start is later than end, assume user error, and check if number is still 1 leap year.
        [InlineData(2022, 2018, 1)]
        public void Calculate_leap_years(int start, int end, int expectedLeapYears)
        {
            Assert.Equal(expectedLeapYears, LeapYears.NumberOfLeapYearsBetween(start, end));
        }
    }
}
