using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.Support;
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
    private readonly CancellationToken _cancellationToken = new();

    public VerifyGitRepositoryInLocalDirectoryActivityTest()
    {
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICachedGitSourceRepository))).Returns(_repository.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IGitManager))).Returns(_gitManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(_configuration.Object);
        _cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(_cacheDb.Object);
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _configuration.Setup(mock => mock.CacheDir).Returns("/cache/dir");

        _repositoryLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _analysisId = new Guid();

        _cacheDb.Setup(mock => mock.RetrieveAnalysis(_analysisId)).ReturnsAsync(
            new CachedAnalysis
            {
                RepositoryUrl = _repositoryLocation,
                RepositoryBranch = "main",
                HistoryInterval = "1m",
                UseCommitHistory = CommitHistory.Full,
                RevisionHistoryMode = RevisionHistoryMode.AllRevisions
            }
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyHandlerFiresEvent()
    {
        var repositoryLocation = new DirectoryInfo(_repositoryLocation);
        repositoryLocation.Create();

        _gitManager.Setup(mock => mock.IsGitRepositoryInitialized(_repositoryLocation)).ReturnsAsync(true);
        _gitManager.Setup(mock => mock.IsWorkingDirectoryClean(_repositoryLocation)).ReturnsAsync(true);
        _gitManager.Setup(mock => mock.GetBranchName(_repositoryLocation)).ReturnsAsync("other-branch");
        _gitManager.Setup(mock => mock.GetRemoteUrl(_repositoryLocation)).ReturnsAsync("git-remote-url");

        var activity = new VerifyGitRepositoryInLocalDirectoryActivity();
        await activity.Handle(_eventEngine.Object, _cancellationToken);

        var expectedCachedGitSource = new CachedGitSource
        {
            Id = new CachedGitSourceId("git-remote-url", "other-branch").Id,
            Url = "git-remote-url",
            Branch = "other-branch",
            LocalPath = repositoryLocation.FullName
        };

        _repository.Verify(mock => mock.Save(It.Is<CachedGitSource>(value =>
            value.Branch == expectedCachedGitSource.Branch &&
            value.Id == expectedCachedGitSource.Id &&
            value.Url == expectedCachedGitSource.Url &&
            value.LocalPath == expectedCachedGitSource.LocalPath
        )));

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<GitRepositoryInLocalDirectoryVerifiedEvent>(value =>
                    value.AnalysisId == _analysisId &&
                    value.HistoryStopData.Path == _repositoryLocation &&
                    value.HistoryStopData.RepositoryId.IsEmpty() == false
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        repositoryLocation.Delete();
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyHandlerFiresFailureEventIfDirectoryDoesNotExist()
    {
        var activity = new VerifyGitRepositoryInLocalDirectoryActivity { AnalysisId = _analysisId };
        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<DirectoryDoesNotExistFailureEvent>(value =>
                    value.ErrorMessage == $"Directory does not exist at {_repositoryLocation}"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyHandlerFiresFailureEventIfDirectoryIsNotGitInitialized()
    {
        var repositoryLocation = new DirectoryInfo(_repositoryLocation);
        repositoryLocation.Create();

        _gitManager.Setup(mock => mock.IsWorkingDirectoryClean(_repositoryLocation)).ReturnsAsync(true);
        _gitManager.Setup(mock => mock.IsGitRepositoryInitialized(_repositoryLocation)).ReturnsAsync(false);

        var activity = new VerifyGitRepositoryInLocalDirectoryActivity { AnalysisId = _analysisId };
        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<DirectoryIsNotGitInitializedFailureEvent>(value =>
                    value.ErrorMessage == $"Directory is not a git initialised directory at {_repositoryLocation}"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        repositoryLocation.Delete();
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task VerifyHandlerFiresFailureEventIfGitWorkingTreeIsNotClean()
    {
        var repositoryLocation = new DirectoryInfo(_repositoryLocation);
        repositoryLocation.Create();

        _gitManager.Setup(mock => mock.IsGitRepositoryInitialized(_repositoryLocation)).ReturnsAsync(true);
        _gitManager.Setup(mock => mock.IsWorkingDirectoryClean(_repositoryLocation)).ReturnsAsync(false);

        var activity = new VerifyGitRepositoryInLocalDirectoryActivity { AnalysisId = _analysisId };
        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<DirectoryIsNotGitInitializedFailureEvent>(value =>
                    value.ErrorMessage == $"There are pending changes in the git directory at {_repositoryLocation}"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );

        repositoryLocation.Delete();
    }
}
