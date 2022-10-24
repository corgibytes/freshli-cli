using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class PrepareCacheForAnalysisActivityTest
{
    private Mock<IApplicationEventEngine> _eventClient;
    private Mock<IServiceProvider> _serviceProvider;
    private Mock<ICacheManager> _cacheManager;
    private string _historyInterval;
    private string _repositoryBranch;
    private string _repositoryUrl;
    private PrepareCacheForAnalysisActivity _activity;

    public PrepareCacheForAnalysisActivityTest()
    {
        _eventClient = new Mock<IApplicationEventEngine>();
        _serviceProvider = new Mock<IServiceProvider>();
        _cacheManager = new Mock<ICacheManager>();

        _historyInterval = "2y";
        _repositoryBranch = "trunk";
        _repositoryUrl = "https://repository.com";

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);

        _activity = new PrepareCacheForAnalysisActivity
        {
            HistoryInterval = _historyInterval,
            RepositoryBranch = _repositoryBranch,
            RepositoryUrl = _repositoryUrl,
            RevisionHistoryMode = RevisionHistoryMode.OnlyLatestRevision,
            UseCommitHistory = CommitHistory.AtInterval
        };
    }

    [Fact]
    public void VerifyItFiresCachePreparedEventWhenPrepareSucceeds()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Returns(true);

        _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.Is<CachePreparedForAnalysisEvent>(value =>
            value.HistoryInterval == _historyInterval &&
            value.RepositoryBranch == _repositoryBranch &&
            value.RepositoryUrl == _repositoryUrl &&
            value.RevisionHistoryMode == RevisionHistoryMode.OnlyLatestRevision &&
            value.UseCommitHistory == CommitHistory.AtInterval
        )));
    }

    [Fact]
    public void VerifyItFiresCachePreparedEventWhenPrepareFails()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Returns(false);

        _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePrepareFailedForAnalysisEvent>()));
    }

    [Fact]
    public void VerifyItFiresCachePreparedEventWhenPrepareThrowsAnException()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Throws(new Exception("failure message"));

        _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.Is<CachePrepareFailedForAnalysisEvent>(value =>
            value.ErrorMessage == "failure message")));
    }

}
