using Corgibytes.Freshli.Cli.Functionality;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[UnitTest]
public class ConfigurationTest
{
    private readonly Configuration _configuration;
    private readonly Mock<IEnvironment> _environment = new();

    public ConfigurationTest()
    {
        _environment.Setup(mock => mock.HomeDirectory).Returns("/path/to/home/dir");
        _configuration = new Configuration(_environment.Object);
    }

    [Fact]
    public void DefaultGitPath() => Assert.Equal("git", _configuration.GitPath);

    [Fact]
    public void DefaultCacheDir() => Assert.Equal("/path/to/home/dir/.freshli", _configuration.CacheDir);
}
