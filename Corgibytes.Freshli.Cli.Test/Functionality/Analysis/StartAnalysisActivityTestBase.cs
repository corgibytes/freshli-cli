using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

public abstract class StartAnalysisActivityTestBase<TActivity, TErrorEvent> where TActivity : IApplicationActivity
    where TErrorEvent : ErrorEvent
{
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    protected readonly Mock<ICacheManager> CacheManager = new();
    protected readonly Mock<IHistoryIntervalParser> IntervalParser = new();
    protected abstract TActivity Activity { get; }

    protected virtual Func<TErrorEvent, bool> EventValidator => _ => true;

    [Fact]
    public void HandlerFiresCacheWasNotPreparedEventWhenCacheIsMissing()
    {
        IntervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);
        CacheManager.Setup(mock => mock.ValidateDirIsCache("example")).Returns(false);

        Activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<TErrorEvent>(value =>
                value.ErrorMessage == "Unable to locate a valid cache directory at: 'example'." &&
                EventValidator(value))));
    }

    [Fact]
    public void HandlerFiresAnalysisStartedEventWhenCacheIsPresent()
    {
        var sampleGuid = new Guid();

        IntervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);

        CacheManager.Setup(mock => mock.ValidateDirIsCache("example")).Returns(true);
        CacheManager.Setup(mock => mock.GetCacheDb("example")).Returns(_cacheDb.Object);
        _cacheDb.Setup(mock => mock.SaveAnalysis(It.IsAny<CachedAnalysis>())).Returns(sampleGuid);

        Activity.Handle(_eventEngine.Object);

        _cacheDb.Verify(mock => mock.SaveAnalysis(It.Is<CachedAnalysis>(value =>
            value.RepositoryUrl == "http://git.example.com" &&
            value.RepositoryBranch == "main" &&
            value.HistoryInterval == "1m"
        )));
        _eventEngine.Verify(mock => mock.Fire(It.Is<AnalysisStartedEvent>(value => value.AnalysisId == sampleGuid)));
    }

    [Fact]
    public void HandlerFiresInvalidHistoryIntervalEventWhenHistoryIntervalValueIsInvalid()
    {
        IntervalParser.Setup(mock => mock.IsValid("1m")).Returns(false);
        CacheManager.Setup(mock => mock.ValidateDirIsCache("example")).Returns(true);

        Activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<InvalidHistoryIntervalEvent>(value =>
                value.ErrorMessage == "The value '1m' is not a valid history interval.")));
    }
}
