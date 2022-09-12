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
    private readonly string? _branch;
    private readonly Mock<ICacheDb> _cacheDb = new();

    private readonly string _cacheDir;
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();

    private readonly string _gitPath;
    private readonly Mock<ICachedGitSourceRepository> _gitSourceRepository = new();
    private readonly string _localPath;
    private readonly string _repositoryId;
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly string _url;

    public CloneGitRepositoryActivityTest()
    {
        _cacheDir = "example";
        _url = "http://git.exaple.com";
        _branch = "main";

        _gitPath = "git";
        _repositoryId = "test";
        _localPath = "test";

        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICachedGitSourceRepository)))
            .Returns(_gitSourceRepository.Object);
        _cacheManager.Setup(mock => mock.GetCacheDb(_cacheDir)).Returns(_cacheDb.Object);
    }

    private void SetupCloneOrPullUsingDefaults() =>
        _gitSourceRepository.Setup(mock => mock.CloneOrPull(_url, _branch, _cacheDir, _gitPath))
            .Returns(new CachedGitSource(_repositoryId, _url, _branch, _localPath));

    [Fact]
    public void HandlerFiresGitRepositoryClonedEventWhenInvokedFromCli()
    {
        SetupCloneOrPullUsingDefaults();

        var activity = new CloneGitRepositoryActivity(_gitSourceRepository.Object, _url, _branch, _cacheDir, _gitPath);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<GitRepositoryClonedEvent>(value =>
                value.AnalysisLocation.RepositoryId == _repositoryId)));
    }

    [Fact]
    public void HandlerFiresCloneGitRepositoryFailedEventWhenCloneFailsAndInvokedFromCli()
    {
        _gitSourceRepository.Setup(mock => mock.CloneOrPull(_url, _branch, _cacheDir, _gitPath))
            .Throws(new GitException("Git clone failed"));

        var activity = new CloneGitRepositoryActivity(_gitSourceRepository.Object, _url, _branch, _cacheDir, _gitPath);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<CloneGitRepositoryFailedEvent>(value => value.ErrorMessage == "Git clone failed")));
    }

    [Fact]
    public void HandlerFiresGitRepositoryClonedEventWhenAnalysisStarted()
    {
        // Saved analysis exists
        var sampleGuid = new Guid();
        const string historyInterval = "1m";
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(sampleGuid))
            .Returns(new CachedAnalysis(_url, _branch, historyInterval));
        SetupCloneOrPullUsingDefaults();

        var activity = new CloneGitRepositoryActivity(_serviceProvider.Object, sampleGuid, _cacheDir, _gitPath);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<GitRepositoryClonedEvent>(value => value.AnalysisId == sampleGuid)));
    }

    [Fact]
    public void HandlerFiresCloneGitRepositoryFailedEventWhenAnalysisIsNotSaved()
    {
        // Saved analysis exists
        var sampleGuid = new Guid();
        const string historyInterval = "1m";
        _cacheDb.Setup(mock => mock.RetrieveAnalysis(sampleGuid))
            .Returns(new CachedAnalysis(_url, _branch, historyInterval));

        _gitSourceRepository.Setup(mock => mock.CloneOrPull(_url, _branch, _cacheDir, _gitPath))
            .Throws(new GitException("Git clone failed"));

        var activity = new CloneGitRepositoryActivity(_serviceProvider.Object, sampleGuid, _cacheDir, _gitPath);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<CloneGitRepositoryFailedEvent>(value => value.ErrorMessage == "Git clone failed")));
    }
}
