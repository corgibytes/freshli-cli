using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiPackageLibYearActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required CachedPackageLibYear PackageLibYear { get; init; }
    public required string AgentExecutablePath { get; init; }

    public int Priority
    {
        get { return 100; }
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = await cacheManager.GetCacheDb();

            var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();

            var historyStopPoint = Parent?.HistoryStopPoint;
            _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

            await resultsApi.CreatePackageLibYear(cacheDb, historyStopPoint.CachedAnalysis.Id, historyStopPoint, PackageLibYear);

            await eventClient.Fire(
                new ApiPackageLibYearCreatedEvent
                {
                    Parent = this,
                    PackageLibYear = PackageLibYear,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Fire(
                new HistoryStopPointProcessingFailedEvent(this, error),
                cancellationToken);
        }
    }
}
