using Corgibytes.Freshli.Cli.Functionality;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[UnitTest]
public class HistoryIntervalParserTest
{
    [Theory]
    [InlineData("1d", true)]
    [InlineData("2w", true)]
    [InlineData("3m", true)]
    [InlineData("4y", true)]
    [InlineData("5c", false)]
    [InlineData("6", true)]
    [InlineData("11d", true)]
    [InlineData("12w", true)]
    [InlineData("13m", true)]
    [InlineData("14y", true)]
    [InlineData("15c", false)]
    [InlineData("16", true)]
    [InlineData("m", false)]
    [InlineData("c", false)]
    [InlineData("-1m", false)]
    [InlineData("-12m", false)]
    [InlineData("0", false)]
    [InlineData("0m", false)]
    [InlineData("1d1d", false)]
    [InlineData("2w2w", false)]
    [InlineData("3m3m", false)]
    [InlineData("4y4y", false)]
    [InlineData("5c5c", false)]
    [InlineData("11d11d", false)]
    [InlineData("12w12w", false)]
    [InlineData("13m13m", false)]
    [InlineData("14y15y", false)]
    [InlineData("15c15c", false)]
    [InlineData("-1m-1m", false)]
    [InlineData("-12m-12m", false)]
    [InlineData("0m0m", false)]
    public void IsValid(string value, bool expectedResult)
    {
        var parser = new HistoryIntervalParser();
        var actualResult = parser.IsValid(value);

        Assert.Equal(expectedResult, actualResult);
    }
}
