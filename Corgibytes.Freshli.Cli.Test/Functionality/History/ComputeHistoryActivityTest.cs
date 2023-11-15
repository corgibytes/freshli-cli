using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Corgibytes.Freshli.Cli.Test.Functionality.Git;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

public class ComputeHistoryActivityTest
{
    private const int HistoryStopPointId = 29;
    private readonly Mock<ICacheDb> _cacheDb = new();
    private readonly Mock<ICacheManager> _cacheManager = new();
    private readonly Mock<IComputeHistory> _computeHistory = new();
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly Mock<IAnalyzeProgressReporter> _progressReporter = new();
    private readonly Mock<ILogger<ComputeHistoryActivity>> _logger = new();
    private readonly CancellationToken _cancellationToken = new();
    private readonly Mock<IResultsApi> _resultsApi = new();
    private readonly string _repositoryUrl = "https://lorem-ipsum.com";
    private readonly string _repositoryBranch = "main";
    private readonly string _repositoryHash;

    public ComputeHistoryActivityTest()
    {
        Configuration = new Configuration(new MockEnvironment());

        _cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(_cacheDb.Object);

        _serviceProvider.Setup(mock => mock.GetService(typeof(IConfiguration))).Returns(Configuration);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ICacheManager))).Returns(_cacheManager.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(_computeHistory.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IAnalyzeProgressReporter)))
            .Returns(_progressReporter.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(ILogger<ComputeHistoryActivity>)))
            .Returns(_logger.Object);
        _serviceProvider.Setup(mock => mock.GetService(typeof(IResultsApi))).Returns(_resultsApi.Object);

        _eventEngine.Setup(mock => mock.ServiceProvider).Returns(_serviceProvider.Object);

        _repositoryHash = new CachedGitSourceId(_repositoryUrl, _repositoryBranch).Id;

        HistoryStopData = new HistoryStopData
        {
            Configuration = Configuration,
            RepositoryId = _repositoryHash,
            CommitId = "abcde1234",
            AsOfDateTime = new DateTimeOffset(2022, 9, 1, 1, 0, 0, TimeSpan.Zero)
        };
    }

    private HistoryStopData HistoryStopData { get; }
    private Configuration Configuration { get; }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task FiresHistoryIntervalStopFoundEvents()
    {
        SetupCachedAnalysis(_repositoryUrl, _repositoryBranch, "1m", CommitHistory.AtInterval,
            RevisionHistoryMode.AllRevisions);
        _resultsApi.Setup(mock => mock.GetDataPoints(_repositoryHash)).ReturnsAsync(new List<HistoryIntervalStop>());

        // Have interval stops available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                DateTime.Parse("2020-01-10T02:21:24+00:00"),
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            ),
            new(
                "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                DateTime.Parse("2020-04-18T05:14:14+00:00"),
                new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
            )
        };
        SetupHistoryStopPointIds(historyIntervalStops);
        _computeHistory.Setup(mock => mock.ComputeWithHistoryInterval(
                It.IsAny<IHistoryStopData>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        await new ComputeHistoryActivity(analysisId, HistoryStopData).Handle(_eventEngine.Object, _cancellationToken);

        // Assert
        VerifyProgressReporting(historyIntervalStops.Count);
        VerifyHistoryStopPoints(analysisId, historyIntervalStops);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task FiresHistoryIntervalStopFoundEventsFilteredViaApi()
    {
        SetupCachedAnalysis(_repositoryUrl, _repositoryBranch, "1m", CommitHistory.AtInterval,
            RevisionHistoryMode.AllRevisions);

        // Have interval stops available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                DateTime.Parse("2020-01-10T02:21:24+00:00"),
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            ),
            new(
                "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                DateTime.Parse("2020-04-18T05:14:14+00:00"),
                new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
            )
        };
        SetupHistoryStopPointIds(historyIntervalStops);
        _computeHistory.Setup(mock => mock.ComputeWithHistoryInterval(
                It.IsAny<IHistoryStopData>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>())
            )
            .Returns(historyIntervalStops);
        _resultsApi.Setup(mock => mock.GetDataPoints(_repositoryHash)).ReturnsAsync(new List<HistoryIntervalStop>()
        {
            new(
                "583d813db3e28b9b44a29db352e2f0e1b4c6e420",
                DateTime.Parse("2020-04-18T05:14:14+00:00"),
                new DateTimeOffset(2021, 5, 19, 15, 24, 24, TimeSpan.Zero)
            )
        });

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        await new ComputeHistoryActivity(analysisId, HistoryStopData).Handle(_eventEngine.Object, _cancellationToken);

        // Assert
        VerifyProgressReporting(1);
        VerifyHistoryStopPoints(analysisId, new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                DateTime.Parse("2020-01-10T02:21:24+00:00"),
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            )
        });
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task FiresHistoryIntervalStopFoundEventsForComputeHistory()
    {
        SetupCachedAnalysis(_repositoryUrl, _repositoryBranch, "1m", CommitHistory.Full,
            RevisionHistoryMode.AllRevisions);
        _resultsApi.Setup(mock => mock.GetDataPoints(_repositoryHash)).ReturnsAsync(new List<HistoryIntervalStop>());

        // Have interval stops available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                DateTimeOffset.Parse("2020-01-10T02:21:24+00:00"),
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            )
        };
        SetupHistoryStopPointIds(historyIntervalStops);
        _computeHistory.Setup(mock => mock.ComputeCommitHistory(
                It.IsAny<IHistoryStopData>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        await new ComputeHistoryActivity(analysisId, HistoryStopData).Handle(_eventEngine.Object, _cancellationToken);

        // Assert
        VerifyProgressReporting(historyIntervalStops.Count);
        VerifyHistoryStopPoints(analysisId, historyIntervalStops);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task FiresHistoryIntervalStopFoundEventsForLatestOnly()
    {
        SetupCachedAnalysis(_repositoryUrl, _repositoryBranch, "1m", CommitHistory.Full,
            RevisionHistoryMode.OnlyLatestRevision);
        _resultsApi.Setup(mock => mock.GetDataPoints(_repositoryHash)).ReturnsAsync(new List<HistoryIntervalStop>());

        // Have interval stop available
        var historyIntervalStops = new List<HistoryIntervalStop>
        {
            new(
                "75c7fcc7336ee718050c4a5c8dfb5598622787b2",
                DateTime.Parse("2020-01-10T02:21:24+00:00"),
                new DateTimeOffset(2021, 2, 20, 12, 31, 34, TimeSpan.Zero)
            )
        };
        SetupHistoryStopPointIds(historyIntervalStops);
        _computeHistory.Setup(mock => mock.ComputeLatestOnly(
                It.IsAny<IHistoryStopData>())
            )
            .Returns(historyIntervalStops);

        // Act
        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        await new ComputeHistoryActivity(analysisId, HistoryStopData).Handle(_eventEngine.Object, _cancellationToken);

        // Assert
        VerifyProgressReporting(historyIntervalStops.Count);
        VerifyHistoryStopPoints(analysisId, historyIntervalStops);
    }

    [Fact(Timeout = Constants.DefaultTestTimeout)]
    public async Task FiresInvalidHistoryIntervalStopEvent()
    {
        // This could happen when we run the analysis on a codebase that barely has any commits.
        // If we want to analyse it, we have to be wary of the interval not being bigger than the age of the first commit.
        // e.g. if it's less than a year old, running the analysis with a 1y interval breaks.

        SetupCachedAnalysis(_repositoryUrl, _repositoryBranch, "1y", CommitHistory.AtInterval,
            RevisionHistoryMode.AllRevisions);
        _resultsApi.Setup(mock => mock.GetDataPoints(_repositoryHash)).ReturnsAsync(new List<HistoryIntervalStop>());

        var listCommits = new MockListCommits();
        listCommits.HasCommitsAvailable(new List<GitCommit>
        {
            new("edd01470c5fb4c5922db060f59bf0e0a5ddce6a5",
                new DateTimeOffset(DateTime.Now.Year, 1, 29, 00, 00, 00, TimeSpan.Zero)),
            new("ca6c6f099e0bb1a63bf5aba7e3db90ba0cff4546",
                new DateTimeOffset(DateTime.Now.Year, 1, 30, 00, 00, 00, TimeSpan.Zero))
        });

        var computeHistory = new ComputeHistory(listCommits, new HistoryIntervalParser());

        _serviceProvider.Setup(mock => mock.GetService(typeof(IComputeHistory))).Returns(computeHistory);

        var analysisId = new Guid("cbc83480-ae47-46de-91df-60747ca8fb09");
        await new ComputeHistoryActivity(analysisId, HistoryStopData).Handle(_eventEngine.Object, _cancellationToken);

        _eventEngine.Verify(mock =>
            mock.Fire(
                It.Is<InvalidHistoryIntervalEvent>(value =>
                    value.ErrorMessage ==
                    "Given range (1y) results in an invalid start date as it occurs before date of oldest commit"
                ),
                _cancellationToken,
                ApplicationTaskMode.Tracked
            )
        );
    }

    private void SetupCachedAnalysis(string repositoryUrl, string repositoryBranch, string historyInterval,
        CommitHistory useCommitHistory, RevisionHistoryMode revisionHistoryMode)
    {
        // Arrange
        // Have an analysis available
        var cachedAnalysis = new CachedAnalysis
        {
            RepositoryUrl = repositoryUrl,
            RepositoryBranch = repositoryBranch,
            HistoryInterval = historyInterval,
            UseCommitHistory = useCommitHistory,
            RevisionHistoryMode = revisionHistoryMode
        };

        _cacheDb.Setup(mock => mock.RetrieveAnalysis(It.IsAny<Guid>())).ReturnsAsync(cachedAnalysis);
    }

    private void SetupHistoryStopPointIds(List<HistoryIntervalStop> historyIntervalStops)
    {
        var stopPointId = HistoryStopPointId;
        foreach (var stopPoint in historyIntervalStops)
        {
            var historyStopPoint = new CachedHistoryStopPoint
            {
                Id = stopPointId,
                GitCommitId = stopPoint.GitCommitIdentifier,
                GitCommitDateTime = stopPoint.GitCommitDateTime,
                AsOfDateTime = stopPoint.AsOfDateTime
            };
            _cacheDb.Setup(mock => mock.AddHistoryStopPoint(It.Is<CachedHistoryStopPoint>(value =>
                    value.GitCommitId == stopPoint.GitCommitIdentifier &&
                    value.GitCommitDateTime == stopPoint.GitCommitDateTime &&
                    value.AsOfDateTime == stopPoint.AsOfDateTime)))
                .ReturnsAsync(historyStopPoint);
            stopPointId++;
        }
    }

    private void VerifyHistoryStopPoints(Guid analysisId, List<HistoryIntervalStop> historyIntervalStops)
    {
        var stopPointId = HistoryStopPointId;
        foreach (var stopPoint in historyIntervalStops)
        {
            var path = Path.Combine(Configuration.CacheDir, "histories", HistoryStopData.RepositoryId,
                stopPoint.GitCommitIdentifier);

            // Verifies that the expected data is being stored in the
            // CachedHistoryStopPoints table.  This is to guard against a bug
            // that was introduced when we started using the cached information
            // from the database instead of passing the HistoryStopData object.
            _cacheDb.Verify(mock => mock.AddHistoryStopPoint(
                It.Is<CachedHistoryStopPoint>(value =>
                    value.CachedAnalysisId == analysisId &&
                    value.RepositoryId == HistoryStopData.RepositoryId &&
                    value.LocalPath == path &&
                    value.GitCommitId == stopPoint.GitCommitIdentifier &&
                    value.GitCommitDateTime == stopPoint.GitCommitDateTime &&
                    value.AsOfDateTime == stopPoint.AsOfDateTime
                )));

            // Verifies that we get an event for each history stop point
            var id = stopPointId;
            _eventEngine.Verify(mock =>
                mock.Fire(
                    It.Is<HistoryIntervalStopFoundEvent>(value =>
                        value.HistoryStopPoint.Id == id
                    ),
                    _cancellationToken,
                    ApplicationTaskMode.Tracked
                )
            );

            stopPointId++;
        }
    }

    private void VerifyProgressReporting(int count)
    {
        _progressReporter.Verify(mock => mock.ReportHistoryStopPointDetectionStarted());
        _progressReporter.Verify(mock => mock.ReportHistoryStopPointDetectionFinished());
        _progressReporter.Verify(mock => mock.ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation.Archive, count));
        _progressReporter.Verify(mock => mock.ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation.Process, count));
    }
}
