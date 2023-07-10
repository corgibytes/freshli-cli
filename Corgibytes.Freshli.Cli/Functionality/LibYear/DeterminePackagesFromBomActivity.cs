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
    public DeterminePackagesFromBomActivity(Guid analysisId, IHistoryStopPointProcessingTask parent, string pathToBom,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        Parent = parent;
        PathToBom = pathToBom;
        AgentExecutablePath = agentExecutablePath;
    }

    public int Priority
    {
        get { return 100; }
    }

    public Guid AnalysisId { get; }
    public IHistoryStopPointProcessingTask Parent { get; }
    public string PathToBom { get; }
    public string AgentExecutablePath { get; }

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
                        Parent = Parent,
                        AgentExecutablePath = AgentExecutablePath,
                        Package = packageUrl
                    },
                    cancellationToken);
            }

            if (packageUrls.Count == 0)
            {
                await eventClient.Fire(new NoPackagesFoundEvent(AnalysisId, Parent), cancellationToken);
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(Parent, error), cancellationToken);
        }
    }
}
