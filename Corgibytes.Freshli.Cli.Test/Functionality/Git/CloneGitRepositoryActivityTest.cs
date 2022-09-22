using System;
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

    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<ICachedGitSourceRepository> _gitSourceRepository = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    private readonly CachedAnalysis _cachedAnalysis;

    public CloneGitRepositoryActivityTest()
    {
        _cacheManager.Setup(mock => mock.GetCacheDb(CacheDir)).Returns(_cacheDb.Object);

        _cachedAnalysis = new CachedAnalysis(Url, Branch, "1m", new CommitHistory());

        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICachedGitSourceRepository)))
            .Returns(_gitSourceRepository.Object);

        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
    }

    private void SetupCloneOrPullUsingDefaults() =>
        _gitSourceRepository.Setup(mock => mock.CloneOrPull(Url, Branch, CacheDir, GitPath))
            .Returns(new CachedGitSource(RepositoryId, Url, Branch, LocalPath));

    private void SetupCachedAnalysis() =>
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(_analysisId)).Returns(_cachedAnalysis);

    [Fact]
    public void HandlerFiresGitRepositoryClonedEventWhenAnalysisStarted()
    {
        SetupCachedAnalysis();
        SetupCloneOrPullUsingDefaults();

        var activity = new CloneGitRepositoryActivity(_analysisId, CacheDir, GitPath);
        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<GitRepositoryClonedEvent>(value => value.AnalysisId == _analysisId)));
    }

    [Fact]
    public void HandlerFiresCloneGitRepositoryFailedEventWhenGitCloneFails()
    {
        SetupCachedAnalysis();

        _gitSourceRepository.Setup(mock => mock.CloneOrPull(Url, Branch, CacheDir, GitPath))
            .Throws(new GitException("Git clone failed"));

        var activity = new CloneGitRepositoryActivity(_analysisId, CacheDir, GitPath);
        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<CloneGitRepositoryFailedEvent>(value => value.ErrorMessage == "Git clone failed")));
    }
}
