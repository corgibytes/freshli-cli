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
    public void ApiServerBaseDefault()
    {
        Assert.Equal("api.freshli.io", Configuration.DefaultApiServerBase);
        Assert.Equal(Configuration.DefaultApiServerBase, _configuration.ApiServerBase);
    }

    [Fact]
    public void ApiServerBaseCanBeSetViaEnvironmentVariable()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.ApiServerBaseEnvVarName))
            .Returns("api.freshli-staging.io:8080");

        Assert.Equal("api.freshli-staging.io:8080", _configuration.ApiServerBase);
    }

    [Fact]
    public void AuthServerBaseDefault()
    {
        Assert.Equal("auth.freshli.io", Configuration.DefaultAuthServerBase);
        Assert.Equal(Configuration.DefaultAuthServerBase, _configuration.AuthServerBase);
    }

    [Fact]
    public void AuthClientIdDefault()
    {
        Assert.Equal("PzGfZ41Df9e0Dk6VpKp2kEI0uhKpggwH", Configuration.DefaultAuthClientId);
        Assert.Equal(Configuration.DefaultAuthClientId, _configuration.AuthClientId);
    }

    [Fact]
    public void CanonicalApiBaseUrl()
    {
        Assert.Equal("https://api.freshli.io/v1", _configuration.CanonicalApiBaseUrl);
    }

    [Fact]
    public void ApiBaseUrlDefault()
    {
        Assert.Equal("https://api.freshli.io/v1", _configuration.ApiBaseUrl);
    }

    [Fact]
    public void ApiBaseUrlDependsOnApiServerBase()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.ApiServerBaseEnvVarName))
            .Returns("api.freshli-staging.io:8888");

        Assert.Equal("https://api.freshli-staging.io:8888/v1", _configuration.ApiBaseUrl);
    }

    [Fact]
    public void UiUrlDefault()
    {
        Assert.Equal("https://freshli.io", _configuration.UiUrl);
    }

    [Fact]
    public void UiUrlCanBeSetViaEnvironmentVariable()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.UiUrlEnvVarName))
            .Returns("https://freshli-staging.io");

        Assert.Equal("https://freshli-staging.io", _configuration.UiUrl);
    }

    [Fact]
    public void UiUrlTrimsTrailingSlashFromEnvironmentVariable()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.UiUrlEnvVarName))
            .Returns("https://freshli-staging.io/");

        Assert.Equal("https://freshli-staging.io", _configuration.UiUrl);
    }
}
