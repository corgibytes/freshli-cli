using System;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[IntegrationTest]
public class CacheManagerTest : IDisposable
{
    private readonly string _tempCacheDir = Path.Combine(Path.GetTempPath(), "Freshli", "CacheManagerTest");
    private readonly Mock<IConfiguration> _configuration = new();

    public CacheManagerTest()
    {
        _configuration.Setup(mock => mock.CacheDir).Returns(_tempCacheDir);
    }

    public void Dispose()
    {
        var tempCacheDirectory = new DirectoryInfo(_tempCacheDir);
        if (tempCacheDirectory.Exists)
        {
            tempCacheDirectory.Delete(true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void SavePersistsACachedAnalysisAndGeneratesAnId()
    {
        var cacheManager = new CacheManager(_configuration.Object);
        cacheManager.Prepare(_tempCacheDir);

        var cache = cacheManager.GetCacheDb();

        var expectedAnalysis = new CachedAnalysis("https://git.example.com", "main", "1m", CommitHistory.Full);

        var id = cache.SaveAnalysis(expectedAnalysis);

        var actualAnalysis = cache.RetrieveAnalysis(id);

        Assert.NotNull(actualAnalysis);
        Assert.Equal(id, actualAnalysis.Id);
        Assert.Equal(expectedAnalysis.RepositoryUrl, actualAnalysis.RepositoryUrl);
        Assert.Equal(expectedAnalysis.RepositoryBranch, actualAnalysis.RepositoryBranch);
        Assert.Equal(expectedAnalysis.HistoryInterval, actualAnalysis.HistoryInterval);
        Assert.Equal(expectedAnalysis.UseCommitHistory, actualAnalysis.UseCommitHistory);
    }
}
