using System.Collections.Generic;
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
    public void GitPathHasADefaultValue() => Assert.Equal("git", _configuration.GitPath);

    [Fact]
    public void GitPathCanBeModified()
    {
        _configuration.GitPath = "/new/git/path";

        Assert.Equal("/new/git/path", _configuration.GitPath);
    }

    [Fact]
    public void CacheDirHasADefaultValue() => Assert.Equal("/path/to/home/dir/.freshli", _configuration.CacheDir);

    [Fact]
    public void CacheDirValueCanBeModified()
    {
        _configuration.CacheDir = "/new/cache/directory";

        Assert.Equal("/new/cache/directory", _configuration.CacheDir);
    }

    [Fact]
    public void FreshliWebApiUrlHasADefaultValue() => Assert.Equal("https://freshli.io", _configuration.FreshliWebApiUrl);

    [Fact]
    public void FreshliWebApiUrlCanBeSetViaEnvironmentVariable()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.FreshliWebApiBaseUrlEnvVarName)).Returns("https://some/other/url");

        Assert.Equal("https://some/other/url", _configuration.FreshliWebApiUrl);
    }

    [Fact]
    public void FreshliWebApiUrlCanBeModified()
    {
        _configuration.FreshliWebApiUrl = "https://yet/another/url";

        Assert.Equal("https://yet/another/url", _configuration.FreshliWebApiUrl);
    }

    [Fact]
    public void FreshliWebUrlCannotBeModifiedWhenEnvironmentVariableIsSet()
    {
        _environment.Setup(mock => mock.GetVariable(Configuration.FreshliWebApiBaseUrlEnvVarName)).Returns("https://url/from/env");

        _configuration.FreshliWebApiUrl = "https://url/from/assignment";

        Assert.Equal("https://url/from/env", _configuration.FreshliWebApiUrl);
    }
}
