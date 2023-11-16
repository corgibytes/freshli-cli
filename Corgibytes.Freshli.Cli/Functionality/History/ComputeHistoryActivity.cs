using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Api;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class ComputeHistoryActivity : ApplicationActivityBase
{
    public readonly IHistoryStopData HistoryStopData;

    public Guid AnalysisId;

    public ComputeHistoryActivity(Guid analysisId, IHistoryStopData historyStopData)
    {
        AnalysisId = analysisId;
        HistoryStopData = historyStopData;
    }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportHistoryStopPointDetectionStarted();

        var computeHistoryService = eventClient.ServiceProvider.GetRequiredService<IComputeHistory>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = await cacheManager.GetCacheDb();
        var cachedAnalysis = await cacheDb.RetrieveAnalysis(AnalysisId);

        if (cachedAnalysis == null)
        {
            await eventClient.Fire(new AnalysisIdNotFoundEvent(), cancellationToken);
            return;
        }

        IEnumerable<HistoryIntervalStop> historyIntervalStops;

        if (cachedAnalysis.RevisionHistoryMode.Equals(RevisionHistoryMode.OnlyLatestRevision))
        {
            historyIntervalStops = computeHistoryService.ComputeLatestOnly(HistoryStopData);
        }
        else if (cachedAnalysis.UseCommitHistory.Equals(CommitHistory.AtInterval))
        {
            try
            {
                historyIntervalStops = computeHistoryService
                    .ComputeWithHistoryInterval(HistoryStopData, cachedAnalysis.HistoryInterval, DateTimeOffset.Now);
            }
            catch (InvalidHistoryIntervalException exception)
            {
                await eventClient.Fire(
                    new InvalidHistoryIntervalEvent { ErrorMessage = exception.Message },
                    cancellationToken);
                return;
            }
        }
        else
        {
            historyIntervalStops = computeHistoryService.ComputeCommitHistory(HistoryStopData);
        }

        // TODO: Filter the list of history interval stops (data points). Any data points that are already on
        // the server should be removed from the list. This will prevent us from re-analyzing the same data.

        var historyIntervalStopsList = historyIntervalStops.ToList();
        var logger = eventClient.ServiceProvider.GetRequiredService<ILogger<ComputeHistoryActivity>>();
        logger.LogDebug("Detected {count} history stop points", historyIntervalStopsList.Count);

        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        var remoteHistoryIntervalStops = await resultsApi.GetDataPoints(HistoryStopData.RepositoryId, cancellationToken);
        logger.LogDebug("API returned {count} history stop points", remoteHistoryIntervalStops.Count);

        var filteredHistoryIntervalStops = historyIntervalStopsList
            .Where(left => !remoteHistoryIntervalStops.Any(right =>
                left.GitCommitIdentifier == right.GitCommitIdentifier &&
                left.AsOfDateTime.Equals(right.AsOfDateTime) &&
                left.GitCommitDateTime.Equals(right.GitCommitDateTime))
            )
            .ToList();
        logger.LogDebug("Filtered down to {count} history stop points", filteredHistoryIntervalStops.Count);

        ReportProgress(progressReporter, filteredHistoryIntervalStops);

        var configuration = eventClient.ServiceProvider.GetRequiredService<IConfiguration>();
        await HandleHistoryIntervalStops(eventClient, filteredHistoryIntervalStops, configuration, cacheDb, cancellationToken);
    }

    private async Task HandleHistoryIntervalStops(IApplicationEventEngine eventClient,
        List<HistoryIntervalStop> historyIntervalStopsList, IConfiguration configuration, ICacheDb cacheDb,
        CancellationToken cancellationToken)
    {
        foreach (var historyIntervalStop in historyIntervalStopsList)
        {
            var historyStop = new HistoryStopData
            {
                Configuration = configuration,
                RepositoryId = HistoryStopData.RepositoryId,
                CommitId = historyIntervalStop.GitCommitIdentifier,
                AsOfDateTime = historyIntervalStop.AsOfDateTime,
                CommittedAt = historyIntervalStop.GitCommitDateTime
            };

            var historyStopPoint = await cacheDb.AddHistoryStopPoint(
                new CachedHistoryStopPoint
                {
                    CachedAnalysisId = AnalysisId,
                    RepositoryId = historyStop.RepositoryId,
                    LocalPath = historyStop.Path,
                    GitCommitId = historyStop.CommitId,
                    GitCommitDateTime = historyStop.CommittedAt!.Value,
                    AsOfDateTime = historyStop.AsOfDateTime
                });

            await eventClient.Fire(
                new HistoryIntervalStopFoundEvent { HistoryStopPoint = historyStopPoint },
                cancellationToken);
        }
    }

    private static void ReportProgress(IAnalyzeProgressReporter progressReporter, List<HistoryIntervalStop> historyIntervalStopsList)
    {
        progressReporter.ReportHistoryStopPointDetectionFinished();

        progressReporter.ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation.Archive,
            historyIntervalStopsList.Count);
        progressReporter.ReportHistoryStopPointsOperationStarted(HistoryStopPointOperation.Process,
            historyIntervalStopsList.Count);
    }
}
