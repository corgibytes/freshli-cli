using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiHistoryStopActivity : IApplicationActivity
{
    public CreateApiHistoryStopActivity(Guid cachedAnalysisId, int historyStopPointId)
    {
        CachedAnalysisId = cachedAnalysisId;
        HistoryStopPointId = historyStopPointId;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid CachedAnalysisId { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public int HistoryStopPointId { get; set; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);
        var cachedAnalysis = cacheDb.RetrieveAnalysis(CachedAnalysisId);
        resultsApi.CreateHistoryPoint(cachedAnalysis!.ApiAnalysisId!.Value, historyStopPoint!.AsOfDateTime);

        eventClient.Fire(new ApiHistoryStopCreatedEvent(CachedAnalysisId, HistoryStopPointId));
    }
}
