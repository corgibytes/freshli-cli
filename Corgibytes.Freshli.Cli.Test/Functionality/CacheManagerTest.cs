using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Microsoft.Data.Sqlite;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[IntegrationTest]
public class CacheManagerTest : IDisposable
{
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly string _tempCacheDir = Path.Combine(Path.GetTempPath(), "Freshli", "CacheManagerTest");

    public CacheManagerTest() => _configuration.Setup(mock => mock.CacheDir).Returns(_tempCacheDir);

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        var tempCacheDirectory = new DirectoryInfo(_tempCacheDir);
        if (tempCacheDirectory.Exists)
        {
            tempCacheDirectory.Delete(true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact(Timeout = Constants.ExpandedTestTimeout)]
    public async Task SavePersistsACachedAnalysisAndGeneratesAnId()
    {
        var cacheManager = new CacheManager(_configuration.Object);
        await cacheManager.Prepare();

        var cache = await cacheManager.GetCacheDb();

        var expectedAnalysis = new CachedAnalysis
        {
            RepositoryUrl = "https://git.example.com",
            RepositoryBranch = "main",
            HistoryInterval = "1m",
            UseCommitHistory = CommitHistory.Full,
            RevisionHistoryMode = RevisionHistoryMode.OnlyLatestRevision
        };

        var id = await cache.SaveAnalysis(expectedAnalysis);

        var actualAnalysis = await cache.RetrieveAnalysis(id);

        Assert.NotNull(actualAnalysis);
        Assert.Equal(id, actualAnalysis.Id);
        Assert.Equal(expectedAnalysis.RepositoryUrl, actualAnalysis.RepositoryUrl);
        Assert.Equal(expectedAnalysis.RepositoryBranch, actualAnalysis.RepositoryBranch);
        Assert.Equal(expectedAnalysis.HistoryInterval, actualAnalysis.HistoryInterval);
        Assert.Equal(expectedAnalysis.UseCommitHistory, actualAnalysis.UseCommitHistory);
        Assert.Equal(expectedAnalysis.RevisionHistoryMode, actualAnalysis.RevisionHistoryMode);
    }
}
