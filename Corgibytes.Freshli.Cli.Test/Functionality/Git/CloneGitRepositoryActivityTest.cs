using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[UnitTest]
public class CloneGitRepositoryActivityTest
{
    private const string CacheDir = "/path/to/cache";
    private const string Url = "http://git.example.com";
    private const string Branch = "test_branch";
    private const string GitPath = "git";
    private const string LocalPath = "/local/path";
    private const string RepositoryId = "test";

    private readonly Guid _analysisId = Guid.NewGuid();

    private readonly CachedAnalysis _cachedAnalysis;
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<ICacheManager> _cacheManager = new();

    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<ICachedGitSourceRepository> _gitSourceRepository = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    private readonly CancellationToken _cancellationToken = new();

    public CloneGitRepositoryActivityTest()
    {
        _configuration.Setup(mock => mock.CacheDir).Returns(CacheDir);
        _configuration.Setup(mock => mock.GitPath).Returns(GitPath);

        _cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(_cacheDb.Object);

        _cachedAnalysis = new CachedAnalysis
        {
            RepositoryUrl = Url,
            RepositoryBranch = Branch,
            HistoryInterval = "1m",
            UseCommitHistory = new CommitHistory(),
            RevisionHistoryMode = RevisionHistoryMode.AllRevisions
        };

        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(_configuration.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICachedGitSourceRepository)))
            .Returns(_gitSourceRepository.Object);

        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

    private void SetupCloneOrPullUsingDefaults() =>
        _gitSourceRepository.Setup(mock => mock.CloneOrPull(Url, Branch))
            .ReturnsAsync(new CachedGitSource
            {
                Id = RepositoryId,
                Url = Url,
                Branch = Branch,
                LocalPath = LocalPath
            });

    private void SetupCachedAnalysis() =>
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(_analysisId)).ReturnsAsync(_cachedAnalysis);

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandlerFiresGitRepositoryClonedEventWhenAnalysisStarted()
    {
        SetupCachedAnalysis();
        SetupCloneOrPullUsingDefaults();

        var activity = new CloneGitRepositoryActivity(_analysisId);
        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<GitRepositoryCloneStartedEvent>(value =>
                    value.AnalysisId == _analysisId
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<GitRepositoryClonedEvent>(value =>
                    value.AnalysisId == _analysisId
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task HandlerFiresCloneGitRepositoryFailedEventWhenGitCloneFails()
    {
        SetupCachedAnalysis();

        _gitSourceRepository.Setup(mock => mock.CloneOrPull(Url, Branch))
            .Throws(new GitException("Git clone failed"));

        var activity = new CloneGitRepositoryActivity(_analysisId);
        await activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<GitRepositoryCloneStartedEvent>(value =>
                    value.AnalysisId == _analysisId
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<CloneGitRepositoryFailedEvent>(value =>
                    value.ErrorMessage == "Git clone failed"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
