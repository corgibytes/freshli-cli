using System;
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
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public PackageURL Package { get; init; } = null!;
    public string AgentExecutablePath { get; init; } = null!;

    public int Priority
    {
        get { return 100; }
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        try
        {
            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath);

            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();
            var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(HistoryStopPointId);

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
                HistoryStopPointId = HistoryStopPointId
            });

            await eventClient.Fire(new LibYearComputedForPackageEvent
            {
                AnalysisId = AnalysisId,
                HistoryStopPointId = HistoryStopPointId,
                PackageLibYearId = packageLibYearId,
                AgentExecutablePath = AgentExecutablePath
            });
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(HistoryStopPointId, error));
        }
    }
}
