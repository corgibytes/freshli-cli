using Corgibytes.Freshli.Cli.Functionality.Api.Auth;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Api.Auth;

[UnitTest]
public class SnakeCaseStringsTest
{
    [Theory]
    [InlineData("", "")]
    [InlineData("test", "test")]
    [InlineData("Test", "test")]
    [InlineData("FirstName", "first_name")]
    [InlineData("ThisIsALongExample", "this_is_a_long_example")]
    public void Valid(string input, string expected)
    {
        Assert.Equal(expected, input.ToSnakeCase());
    }
}
