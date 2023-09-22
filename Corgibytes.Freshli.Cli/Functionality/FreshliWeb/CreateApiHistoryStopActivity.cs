using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiHistoryStopActivity : IApplicationActivity
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public required Guid CachedAnalysisId { get; init; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public required CachedHistoryStopPoint HistoryStopPoint { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        await resultsApi.CreateHistoryPoint(cacheDb, CachedAnalysisId, HistoryStopPoint);

        await eventClient.Fire(
            new ApiHistoryStopCreatedEvent { CachedAnalysisId = CachedAnalysisId, HistoryStopPoint = HistoryStopPoint },
            cancellationToken
        );
    }
}
