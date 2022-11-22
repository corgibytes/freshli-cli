using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

public class HistoryStopDataTest
{
    private const string CacheDirectory = "/path/to/cache/dir";
    private readonly Mock<IConfiguration> _configuration = new();

    public HistoryStopDataTest() => _configuration.Setup(mock => mock.CacheDir).Returns(CacheDirectory);

    [Fact]
    public void PathWithoutCommitId()
    {
        var repositoryId = Guid.NewGuid().ToString();
        var data = new HistoryStopData(_configuration.Object, repositoryId);

        var expectedPath = Path.Combine(CacheDirectory, "repositories", repositoryId);
        Assert.Equal(expectedPath, data.Path);
    }

    [Fact]
    public void PathWithCommitId()
    {
        var repositoryId = Guid.NewGuid().ToString();
        var commitId = Guid.NewGuid().ToString();
        var data = new HistoryStopData(_configuration.Object, repositoryId, commitId);

        var expectedPath = Path.Combine(CacheDirectory, "histories", repositoryId, commitId);
        Assert.Equal(expectedPath, data.Path);
    }

    [Fact]
    public void PathUsingLocalDirectory()
    {
        var expectedPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var repositoryId = Guid.NewGuid().ToString();

        var historyStopData =
            new HistoryStopData(_configuration.Object, repositoryId) { LocalDirectory = expectedPath };

        Assert.Equal(expectedPath, historyStopData.Path);
    }

    [Fact]
    public void PathUsingLocalDirectoryWithCommitId()
    {
        var localDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var repositoryId = Guid.NewGuid().ToString();
        var commitId = Guid.NewGuid().ToString();

        var historyStopData =
            new HistoryStopData(_configuration.Object, repositoryId, commitId) { LocalDirectory = localDirectory };

        var expectedPath = Path.Combine(CacheDirectory, "histories", repositoryId, commitId);
        Assert.Equal(expectedPath, historyStopData.Path);
    }
}
