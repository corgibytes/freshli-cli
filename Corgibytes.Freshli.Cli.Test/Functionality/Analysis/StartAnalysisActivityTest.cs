using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

public class StartAnalysisActivityTest
{
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IHistoryIntervalParser> _intervalParser = new();

    private StartAnalysisActivity Activity => new(_cacheManager.Object, _intervalParser.Object)
    {
        CacheDirectory = "example",
        RepositoryUrl = "http://git.example.com",
        RepositoryBranch = "main",
        HistoryInterval = "1m"
    };

    [Fact]
    public void HandlerFiresCacheWasNotPreparedEventWhenCacheIsMissing()
    {
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);
        _cacheManager.Setup(mock => mock.ValidateDirIsCache("example")).Returns(false);

        Activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<CacheWasNotPreparedEvent>(value =>
                value.ErrorMessage == "Unable to locate a valid cache directory at: 'example'.")));
    }

    [Fact]
    public void HandlerFiresAnalysisStartedEventWhenCacheIsPresent()
    {
        var sampleGuid = new Guid();

        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(true);

        _cacheManager.Setup(mock => mock.ValidateDirIsCache("example")).Returns(true);
        _cacheManager.Setup(mock => mock.GetCacheDb("example")).Returns(_cacheDb.Object);
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
        _intervalParser.Setup(mock => mock.IsValid("1m")).Returns(false);
        _cacheManager.Setup(mock => mock.ValidateDirIsCache("example")).Returns(true);

        Activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<InvalidHistoryIntervalEvent>(value =>
                value.ErrorMessage == "The value '1m' is not a valid history interval.")));
    }
}
