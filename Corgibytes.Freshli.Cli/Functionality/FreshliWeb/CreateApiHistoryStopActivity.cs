using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiHistoryStopActivity : IApplicationActivity
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public required CachedHistoryStopPoint HistoryStopPoint { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = await cacheManager.GetCacheDb();

        // TODO: Why is the cacheDb passed in this way. Shouldn't the ResultsAPI received the ICacheManager instance via dependency injection and then get the cacheDb from that?
        await resultsApi.CreateHistoryPoint(cacheDb, HistoryStopPoint.CachedAnalysis.Id, HistoryStopPoint);

        await eventClient.Fire(
            new ApiHistoryStopCreatedEvent { HistoryStopPoint = HistoryStopPoint },
            cancellationToken
        );
    }
}
