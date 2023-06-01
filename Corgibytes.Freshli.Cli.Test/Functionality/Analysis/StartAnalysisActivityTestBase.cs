using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
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
    private readonly Mock<IHistoryStopPointProcessingTask> _parent = new();
    private readonly CancellationToken _cancellationToken = new(false);

    protected StartAnalysisActivityTestBase()
    {
        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(_configuration.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IHistoryIntervalParser))).Returns(_intervalParser.Object);
    }

    protected abstract TActivity Activity { get; }

    protected virtual Func<TErrorEvent, bool> EventValidator => _ => true;

    [Fact(Timeout = 500)]
    public async Task HandlerFiresCacheWasNotPreparedEventWhenCacheIsMissing()
    {
        _configuration.Setup(mock => mock.CacheDir).Returns("example");
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);
        _cacheManager.Setup(mock => mock.ValidateCacheDirectory()).ReturnsAsync(false);

        await Activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<TErrorEvent>(value =>
                    value.ErrorMessage == "Unable to locate a valid cache directory at: 'example'." &&
                    EventValidator(value)
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = 500)]
    public async Task HandlerFiresAnalysisStartedEventWhenCacheIsPresent()
    {
        var analysisId = Guid.NewGuid();

        _configuration.Setup(mock => mock.CacheDir).Returns("example");
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);

        _cacheManager.Setup(mock => mock.ValidateCacheDirectory()).ReturnsAsync(true);
        _cacheManager.Setup(mock => mock.GetCacheDb()).Returns(_cacheDb.Object);
        _cacheDb.Setup(mock => mock.SaveAnalysis(It.IsAny<CachedAnalysis>())).ReturnsAsync(analysisId);

        await Activity.Handle(_eventEngine.Object, _cancellationToken);

        _cacheDb.Verify(mock => mock.SaveAnalysis(It.Is<CachedAnalysis>(value =>
            value.RepositoryUrl == "http://git.example.com" &&
            value.RepositoryBranch == "main" &&
            value.HistoryInterval == "1m"
        )));
        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<AnalysisStartedEvent>(
                    value => value.AnalysisId == analysisId
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    [Fact(Timeout = 500)]
    public async Task HandlerFiresInvalidHistoryIntervalEventWhenHistoryIntervalValueIsInvalid()
    {
        _configuration.Setup(mock => mock.CacheDir).Returns("example");
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(false);
        _cacheManager.Setup(mock => mock.ValidateCacheDirectory()).ReturnsAsync(true);

        await Activity.Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(
            mock => mock.Fire(
                It.Is<InvalidHistoryIntervalEvent>(
                    value => value.ErrorMessage == "The value '1m' is not a valid history interval."
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }
}
