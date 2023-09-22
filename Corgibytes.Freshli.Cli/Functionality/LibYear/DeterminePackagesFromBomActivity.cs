using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class DeterminePackagesFromBomActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public int Priority
    {
        get { return 100; }
    }

    public required Guid AnalysisId { get; init; }
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string PathToBom { get; init; }
    public required string AgentExecutablePath { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var logger = eventClient.ServiceProvider.GetService<ILogger<DeterminePackagesFromBomActivity>>();
        logger?.LogTrace("Handling retrieval of packageUrls from BomFile = {PathToBom}", PathToBom);

        try
        {
            var bomReader = eventClient.ServiceProvider.GetRequiredService<IBomReader>();
            var packageUrls = bomReader.AsPackageUrls(PathToBom);
            logger?.LogTrace("Received {Count}  packageUrls from BomFile = {PathToBom}",
                packageUrls.Count, PathToBom);

            foreach (var packageUrl in packageUrls)
            {
                if (packageUrl == null)
                {
                    throw new Exception($"Null package URL detected for in {PathToBom}");
                }

                await eventClient.Fire(
                    new PackageFoundEvent
                    {
                        AnalysisId = AnalysisId,
                        Parent = this,
                        AgentExecutablePath = AgentExecutablePath,
                        Package = packageUrl
                    },
                    cancellationToken
                );
            }

            if (packageUrls.Count == 0)
            {
                await eventClient.Fire(new NoPackagesFoundEvent(AnalysisId, this), cancellationToken);
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(this, error), cancellationToken);
        }
    }
}
