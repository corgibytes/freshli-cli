using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

[UnitTest]
public class PrepareCacheForAnalysisActivityTest
{
    private readonly PrepareCacheForAnalysisActivity _activity;
    private readonly Mock<ICacheManager> _cacheManager;
    private readonly Mock<IApplicationEventEngine> _eventClient;
    private readonly string _historyInterval;
    private readonly string _repositoryBranch;
    private readonly string _repositoryUrl;

    public PrepareCacheForAnalysisActivityTest()
    {
        _eventClient = new Mock<IApplicationEventEngine>();
        var serviceProvider = new Mock<IServiceProvider>();
        _cacheManager = new Mock<ICacheManager>();

        _historyInterval = "2y";
        _repositoryBranch = "trunk";
        _repositoryUrl = "https://repository.com";

        _eventClient.Setup(mock => mock.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);

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
    public async Task VerifyItFiresCachePreparedEventWhenPrepareSucceeds()
    {
        _cacheManager.Setup(mock => mock.Prepare()).ReturnsAsync(true);

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.Is<CachePreparedForAnalysisEvent>(value =>
            value.HistoryInterval == _historyInterval &&
            value.RepositoryBranch == _repositoryBranch &&
            value.RepositoryUrl == _repositoryUrl &&
            value.RevisionHistoryMode == RevisionHistoryMode.OnlyLatestRevision &&
            value.UseCommitHistory == CommitHistory.AtInterval
        )));
    }

    [Fact]
    public async Task VerifyItFiresCachePreparedEventWhenPrepareFails()
    {
        _cacheManager.Setup(mock => mock.Prepare()).ReturnsAsync(false);

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.IsAny<CachePrepareFailedForAnalysisEvent>()));
    }

    [Fact]
    public async Task VerifyItFiresCachePreparedEventWhenPrepareThrowsAnException()
    {
        _cacheManager.Setup(mock => mock.Prepare()).Throws(new Exception("failure message"));

        await _activity.Handle(_eventClient.Object);

        _eventClient.Verify(mock => mock.Fire(It.Is<CachePrepareFailedForAnalysisEvent>(value =>
            value.ErrorMessage == "failure message")));
    }
}
