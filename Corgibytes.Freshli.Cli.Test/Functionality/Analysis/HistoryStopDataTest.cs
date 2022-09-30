using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

public class HistoryStopDataTest
{
    private readonly string _cacheDirectory = "/path/to/cache/dir";
    private readonly Mock<IConfiguration> _configuration = new();

    public HistoryStopDataTest() => _configuration.Setup(mock => mock.CacheDir).Returns(_cacheDirectory);

    [Fact]
    public void PathWithoutCommitId()
    {
        var repositoryId = Guid.NewGuid().ToString();
        var data = new HistoryStopData(_configuration.Object, repositoryId);

        var expectedPath = Path.Combine(_cacheDirectory, "repositories", repositoryId);
        Assert.Equal(expectedPath, data.Path);
    }

    [Fact]
    public void PathWithCommitId()
    {
        var repositoryId = Guid.NewGuid().ToString();
        var commitId = Guid.NewGuid().ToString();
        var data = new HistoryStopData(_configuration.Object, repositoryId, commitId);

        var expectedPath = Path.Combine(_cacheDirectory, "histories", repositoryId, commitId);
        Assert.Equal(expectedPath, data.Path);
    }
}
