using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiPackageLibYearActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public required Guid AnalysisId { get; init; }
    public required IHistoryStopPointProcessingTask Parent { get; init; }
    public required int PackageLibYearId { get; init; }
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
            var cacheDb = cacheManager.GetCacheDb();

            var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
            await resultsApi.CreatePackageLibYear(cacheDb, AnalysisId, PackageLibYearId);

            await eventClient.Fire(
                new ApiPackageLibYearCreatedEvent
                {
                    AnalysisId = AnalysisId,
                    Parent = Parent,
                    PackageLibYearId = PackageLibYearId,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Fire(
                new HistoryStopPointProcessingFailedEvent(Parent, error),
                cancellationToken);
        }
    }
}
