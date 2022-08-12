using System;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[IntegrationTest]
public class CacheManagerTest : IDisposable
{
    private readonly string _tempCacheDir;

    public CacheManagerTest() => _tempCacheDir = Path.Combine(Path.GetTempPath(), "Freshli", "CacheManagerTest");

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
        var cacheManager = new CacheManager();
        cacheManager.Prepare(_tempCacheDir);

        var cache = cacheManager.GetCacheDb(_tempCacheDir);

        var expectedAnalysis = new CachedAnalysis("https://git.example.com", "main", "1m");

        var id = cache.SaveAnalysis(expectedAnalysis);

        var actualAnalysis = cache.RetrieveAnalysis(id);

        Assert.NotNull(actualAnalysis);
        Assert.Equal(id, actualAnalysis.Id);
        Assert.Equal(expectedAnalysis.RepositoryUrl, actualAnalysis.RepositoryUrl);
        Assert.Equal(expectedAnalysis.RepositoryBranch, actualAnalysis.RepositoryBranch);
        Assert.Equal(expectedAnalysis.HistoryInterval, actualAnalysis.HistoryInterval);
    }
}
