using System.IO;

using Xunit;

using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Test.Services;

[IntegrationTest]
public class FileValidatorTest
{
    [Fact]
    public void IsValidFilePathWithValidFilePath()
    {
        var tempFilePath = Path.GetTempFileName();

        var validator = new FileValidator();
        Assert.True(validator.IsValidFilePath(tempFilePath));

        File.Delete(tempFilePath);
    }

    [Fact]
    public void IsValidFilePathWithNull()
    {
        var validator = new FileValidator();
        Assert.False(validator.IsValidFilePath(null!));
    }

    [Fact]
    public void IsValidFilePathWithEmptyString()
    {
        var validator = new FileValidator();
        Assert.False(validator.IsValidFilePath(string.Empty));
    }

    [Fact]
    public void IsValidFilePathWithBogusFilePath()
    {
        var validator = new FileValidator();
        Assert.False(validator.IsValidFilePath("bogus"));
    }
}
