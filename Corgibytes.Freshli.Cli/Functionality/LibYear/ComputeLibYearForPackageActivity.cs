using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class ComputeLibYearForPackageActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public required Guid AnalysisId { get; init; }
    public required IHistoryStopPointProcessingTask Parent { get; init; }
    public required PackageURL Package { get; init; }
    public required string AgentExecutablePath { get; init; }

    public int Priority
    {
        get { return 100; }
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath);

            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();
            var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(Parent.HistoryStopPointId);

            var calculator = eventClient.ServiceProvider.GetRequiredService<IPackageLibYearCalculator>();
            var packageLibYear = await calculator.ComputeLibYear(agentReader, Package, historyStopPoint!.AsOfDateTime);

            var packageLibYearId = await cacheDb.AddPackageLibYear(new CachedPackageLibYear
            {
                PackageName = Package.Name,
                CurrentVersion = packageLibYear.CurrentVersion?.ToString(),
                ReleaseDateCurrentVersion = packageLibYear.ReleaseDateCurrentVersion,
                LatestVersion = packageLibYear.LatestVersion?.ToString(),
                ReleaseDateLatestVersion = packageLibYear.ReleaseDateLatestVersion,
                LibYear = packageLibYear.LibYear,
                HistoryStopPointId = Parent.HistoryStopPointId
            });

            await eventClient.Fire(
                new LibYearComputedForPackageEvent
                {
                    AnalysisId = AnalysisId,
                    Parent = Parent,
                    PackageLibYearId = packageLibYearId,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(Parent, error), cancellationToken);
        }
    }
}
