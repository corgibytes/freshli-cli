using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

public class CachedGitSourceRepositoryTest
{
    [Fact]
    public async Task CloneOrPullRetrievesBranchNameOnCloneWhenBranchIsNull()
    {
        var commandInvoker = new CommandInvoker();
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(mock => mock.GitPath).Returns("git");
        var cacheManager = new Mock<ICacheManager>();
        var cacheDb = new Mock<ICacheDb>();
        cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(cacheDb.Object);
        cacheDb.Setup(mock => mock.RetrieveCachedGitSource(It.IsAny<CachedGitSourceId>())).ReturnsAsync(null as CachedGitSource);
        var repository = new CachedGitSourceRepository(commandInvoker, configuration.Object, cacheManager.Object);

        var tempPath = Path.GetTempFileName();
        File.Delete(tempPath);
        Directory.CreateDirectory(tempPath);

        cacheManager.Setup(mock => mock.GetDirectoryInCache(new [] { "repositories", It.IsAny<string>() })).ReturnsAsync(new DirectoryInfo(tempPath));

        var repositoryUrl = "https://github.com/corgibytes/freshli-fixture-java-test";

        var cachedGitSource = await repository.CloneOrPull(repositoryUrl, null);

        Assert.Equal("main", cachedGitSource.Branch);
        Assert.Equal(repositoryUrl, cachedGitSource.Url);

        cacheDb.Verify(mock => mock.AddCachedGitSource(It.Is<CachedGitSource>(source =>
            source.Url == repositoryUrl &&
            source.Branch == "main")));
    }
}
