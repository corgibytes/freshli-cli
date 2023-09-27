using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Support;

[UnitTest]
public class ConfigurationTest
{
    private readonly Configuration _configuration;
    private readonly Mock<IEnvironment> _environment = new();
    private readonly string _homeRootPath = Path.Combine(Path.DirectorySeparatorChar.ToString(), "path", "to", "home", "dir");

    public ConfigurationTest()
    {
        _environment.Setup(mock => mock.HomeDirectory).Returns(_homeRootPath);
        _configuration = new Configuration(_environment.Object);
    }

    [Fact]
    public void GitPathHasADefaultValue() => Assert.Equal("git", _configuration.GitPath);

    [Fact]
    public void GitPathCanBeModified()
    {
        var newGitPath = Path.Combine(Path.DirectorySeparatorChar.ToString(), "new", "git", "path");
        _configuration.GitPath = newGitPath;

        Assert.Equal(newGitPath, _configuration.GitPath);
    }

    [Fact]
    public void CacheDirHasADefaultValue() => Assert.Equal(
        Path.Combine(_homeRootPath, ".freshli"), _configuration.CacheDir);

    [Fact]
    public void CacheDirValueCanBeModified()
    {
        var newCacheDir = Path.Combine(Path.DirectorySeparatorChar.ToString(), "new", "cache", "directory");
        _configuration.CacheDir = newCacheDir;

        Assert.Equal(newCacheDir, _configuration.CacheDir);
    }

    [Fact]
    public void FreshliWebApiBaseUrlHasADefaultValue() =>
        Assert.Equal("https://freshli.io", _configuration.FreshliWebApiBaseUrl);

    [Fact]
    public void FreshliWebApiBaseUrlCanBeSetViaEnvironmentVariable()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.FreshliWebApiBaseUrlEnvVarName))
            .Returns("https://some/other/url");

        Assert.Equal("https://some/other/url", _configuration.FreshliWebApiBaseUrl);
    }

    [Fact]
    public void FreshliWebApiBaseUrlCanBeModified()
    {
        _configuration.FreshliWebApiBaseUrl = "https://yet/another/url";

        Assert.Equal("https://yet/another/url", _configuration.FreshliWebApiBaseUrl);
    }

    [Fact]
    public void FreshliWebUrlCannotBeModifiedWhenEnvironmentVariableIsSet()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.FreshliWebApiBaseUrlEnvVarName))
            .Returns("https://url/from/env");

        _configuration.FreshliWebApiBaseUrl = "https://url/from/assignment";

        Assert.Equal("https://url/from/env", _configuration.FreshliWebApiBaseUrl);
    }

    [Fact]
    public void FreshliWebUrlSetRemovesTrailingSlash()
    {
        _configuration.FreshliWebApiBaseUrl = "https://url/with/a/trailing/slash/";

        Assert.Equal("https://url/with/a/trailing/slash", _configuration.FreshliWebApiBaseUrl);
    }

    [Fact]
    public void FreshliWebUrlRemovesTrailingSlashFromEnvVariable()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.FreshliWebApiBaseUrlEnvVarName))
            .Returns("https://url/with/a/trailing/slash/");

        Assert.Equal("https://url/with/a/trailing/slash", _configuration.FreshliWebApiBaseUrl);
    }
}
