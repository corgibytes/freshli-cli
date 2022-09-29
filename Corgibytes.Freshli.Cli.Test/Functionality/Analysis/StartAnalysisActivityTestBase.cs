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
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IHistoryIntervalParser> _intervalParser = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    protected StartAnalysisActivityTestBase()
    {
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(_configuration.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IHistoryIntervalParser))).Returns(_intervalParser.Object);
    }

    protected abstract TActivity Activity { get; }

    protected virtual Func<TErrorEvent, bool> EventValidator => _ => true;

    [Fact]
    public void HandlerFiresCacheWasNotPreparedEventWhenCacheIsMissing()
    {
        _configuration.Setup(mock => mock.CacheDir).Returns("example");
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);
        _cacheManager.Setup(mock => mock.ValidateCacheDirectory()).Returns(false);

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

        _configuration.Setup(mock => mock.CacheDir).Returns("example");
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);

        _cacheManager.Setup(mock => mock.ValidateCacheDirectory()).Returns(true);
        _cacheManager.Setup(mock => mock.GetCacheDb()).Returns(_cacheDb.Object);
        _cacheDb.Setup(mock => mock.SaveAnalysis(It.IsAny<CachedAnalysis>())).Returns(sampleGuid);

        Activity.Handle(_eventEngine.Object);

        _cacheDb.Verify(mock => mock.SaveAnalysis(It.Is<CachedAnalysis>(value =>
            value.RepositoryUrl == "http://git.example.com" &&
            value.RepositoryBranch == "main" &&
            value.HistoryInterval == "1m"
        )));
        _eventEngine.Verify(mock => mock.Fire(It.Is<AnalysisStartedEvent>(value =>
            value.AnalysisId == sampleGuid &&
            value.RepositoryUrl == "http://git.example.com"
            )));
    }

    [Fact]
    public void HandlerFiresInvalidHistoryIntervalEventWhenHistoryIntervalValueIsInvalid()
    {
        _configuration.Setup(mock => mock.CacheDir).Returns("example");
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(false);
        _cacheManager.Setup(mock => mock.ValidateCacheDirectory()).Returns(true);

        Activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<InvalidHistoryIntervalEvent>(value =>
                value.ErrorMessage == "The value '1m' is not a valid history interval.")));
    }
}
