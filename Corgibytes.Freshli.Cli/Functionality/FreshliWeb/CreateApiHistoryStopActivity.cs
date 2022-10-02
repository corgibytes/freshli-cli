using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiHistoryStopActivity : IApplicationActivity
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid CachedAnalysisId { get; set; }
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public IHistoryStopData HistoryStopData { get; set; }

    public CreateApiHistoryStopActivity(Guid cachedAnalysisId, IHistoryStopData historyStopData)
    {
        CachedAnalysisId = cachedAnalysisId;
        HistoryStopData = historyStopData;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        var cachedAnalysis = cacheDb.RetrieveAnalysis(CachedAnalysisId);
        resultsApi.CreateHistoryPoint(cachedAnalysis!.ApiAnalysisId!.Value, HistoryStopData.Moment!.Value);

        eventClient.Fire(new ApiHistoryStopCreatedEvent(CachedAnalysisId, HistoryStopData));
    }
}
