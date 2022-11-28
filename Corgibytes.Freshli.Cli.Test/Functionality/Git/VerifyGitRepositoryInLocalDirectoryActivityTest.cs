using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using ServiceStack;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[UnitTest]
public class VerifyGitRepositoryInLocalDirectoryActivityTest
{
    private readonly Guid _analysisId;
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IGitManager> _gitManager = new();
    private readonly Mock<ICachedGitSourceRepository> _repository = new();
    private readonly string _repositoryLocation;
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    public VerifyGitRepositoryInLocalDirectoryActivityTest()
    {
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICachedGitSourceRepository))).Returns(_repository.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IGitManager))).Returns(_gitManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(_configuration.Object);
        _cacheManager.Setup(mock => mock.GetCacheDb()).Returns(_cacheDb.Object);
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _configuration.Setup(mock => mock.CacheDir).Returns("/cache/dir");

        _repositoryLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _analysisId = new Guid();

        _cacheDb.Setup(mock => mock.RetrieveAnalysis(_analysisId)).ReturnsAsync(
            new CachedAnalysis(_repositoryLocation, "main", "1m", CommitHistory.Full, RevisionHistoryMode.AllRevisions)
        );
    }

    [Fact]
    public async Task VerifyHandlerFiresEvent()
    {
        var repositoryLocation = new DirectoryInfo(_repositoryLocation);
        repositoryLocation.Create();

        _gitManager.Setup(mock =>
            mock.IsGitRepositoryInitialized(_repositoryLocation)).ReturnsAsync(true);

        var activity = new VerifyGitRepositoryInLocalDirectoryActivity();
        await activity.Handle(_eventEngine.Object);

        var expectedCachedGitSource = new CachedGitSource(
            new CachedGitSourceId(repositoryLocation.FullName).Id, _repositoryLocation, null,
            repositoryLocation.FullName
        );

        _repository.Verify(mock => mock.Save(It.Is<CachedGitSource>(value =>
            value.Branch == expectedCachedGitSource.Branch &&
            value.Id == expectedCachedGitSource.Id &&
            value.Url == expectedCachedGitSource.Url &&
            value.LocalPath == expectedCachedGitSource.LocalPath
        )));

        _eventEngine.Verify(mock => mock.Fire(It.Is<GitRepositoryInLocalDirectoryVerifiedEvent>(value =>
            value.AnalysisId == _analysisId &&
            value.HistoryStopData.Path == _repositoryLocation &&
            value.HistoryStopData.RepositoryId.IsEmpty() == false
        )));

        repositoryLocation.Delete();
    }

    [Fact]
    public async Task VerifyHandlerFiresFailureEventIfDirectoryDoesNotExist()
    {
        var activity = new VerifyGitRepositoryInLocalDirectoryActivity { AnalysisId = _analysisId };
        await activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock => mock.Fire(It.Is<DirectoryDoesNotExistFailureEvent>(value =>
            value.ErrorMessage == $"Directory does not exist at {_repositoryLocation}"
        )));
    }

    [Fact]
    public async Task VerifyHandlerFiresFailureEventIfDirectoryIsNotGitInitialized()
    {
        var repositoryLocation = new DirectoryInfo(_repositoryLocation);
        repositoryLocation.Create();

        _gitManager.Setup(mock => mock.IsGitRepositoryInitialized(_repositoryLocation))
            .ReturnsAsync(false);

        var activity = new VerifyGitRepositoryInLocalDirectoryActivity { AnalysisId = _analysisId };
        await activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock => mock.Fire(It.Is<DirectoryIsNotGitInitializedFailureEvent>(value =>
            value.ErrorMessage == $"Directory is not a git initialised directory at {_repositoryLocation}"
        )));

        repositoryLocation.Delete();
    }
}
