using System;
using System.Threading;
using System.Threading.Tasks;
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

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        await resultsApi.CreateHistoryPoint(cacheDb, CachedAnalysisId, HistoryStopPointId);

        await eventClient.Fire(new ApiHistoryStopCreatedEvent(CachedAnalysisId, HistoryStopPointId), cancellationToken);
    }
}
