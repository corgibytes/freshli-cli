using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoSmart.AsyncLock;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class ComputeLibYearForPackageActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required PackageURL Package { get; init; }
    public required string AgentExecutablePath { get; init; }
    private static readonly AsyncLock s_cacheLibYearLock = new();

    public int Priority
    {
        get { return 100; }
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var historyStopPoint = Parent?.HistoryStopPoint;
            _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

            var manifest = Parent?.Manifest;
            _ = manifest ?? throw new Exception("Parent's Manifest is null");

            var logger = eventClient.ServiceProvider.GetService<ILogger<ComputeLibYearForPackageActivity>>();

            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath, cancellationToken);

            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = await cacheManager.GetCacheDb();

            CachedPackageLibYear? packageLibYear;
            using (await s_cacheLibYearLock.LockAsync(cancellationToken))
            {
                packageLibYear = await cacheDb.RetrievePackageLibYear(Package, historyStopPoint.AsOfDateTime);
                if (packageLibYear != null)
                {
                    if (!packageLibYear.DoesBelongTo(manifest))
                    {
                        await cacheDb.AddPackageLibYear(manifest, packageLibYear);
                    }
                }
                else
                {
                    var calculator =
                        agentManager.GetLibYearCalculator(agentReader, Package, historyStopPoint.AsOfDateTime);
                    var computedPackageLibYear = await calculator.ComputeLibYear();
                    if (computedPackageLibYear == null)
                    {
                        logger?.LogWarning("Failed to compute libyear for {Package} as of {Time}", Package,
                            historyStopPoint.AsOfDateTime);
                        return;
                    }

                    packageLibYear = await cacheDb.AddPackageLibYear(
                        manifest,
                        new CachedPackageLibYear
                        {
                            PackageUrl = Package.ToString()!,
                            AsOfDateTime = historyStopPoint.AsOfDateTime,
                            ReleaseDateCurrentVersion = computedPackageLibYear.ReleaseDateCurrentVersion,
                            LatestVersion = computedPackageLibYear.LatestVersion.ToString(),
                            ReleaseDateLatestVersion = computedPackageLibYear.ReleaseDateLatestVersion,
                            LibYear = computedPackageLibYear.LibYear
                        }
                    );
                }
            }

            await eventClient.Fire(
                new LibYearComputedForPackageEvent
                {
                    Parent = this,
                    PackageLibYear = packageLibYear,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(this, error), cancellationToken);
        }
    }

    public override string ToString()
    {
        var historyStopPointId = Parent?.HistoryStopPoint?.Id ?? 0;

        var manifestId = Parent?.Manifest?.Id ?? 0;
        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name} - AgentExecutablePath = {AgentExecutablePath}, Manifest = {manifestId}, PackageUrl = {Package}";
    }
}
