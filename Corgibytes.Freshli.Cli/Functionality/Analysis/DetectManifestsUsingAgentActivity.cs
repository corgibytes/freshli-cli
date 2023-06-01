using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class DetectManifestsUsingAgentActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public DetectManifestsUsingAgentActivity(Guid analysisId, IHistoryStopPointProcessingTask parent,
        string agentExecutablePath)
    {
        AnalysisId = analysisId;
        Parent = parent;
        AgentExecutablePath = agentExecutablePath;
    }

    public int Priority
    {
        get { return 100; }
    }

    public Guid AnalysisId { get; }
    public string AgentExecutablePath { get; }
    public IHistoryStopPointProcessingTask Parent { get; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
            var agentReader = agentManager.GetReader(AgentExecutablePath, cancellationToken);

            var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
            var cacheDb = cacheManager.GetCacheDb();
            var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(Parent.HistoryStopPointId);

            var manifestsFound = false;
            await foreach (var manifestPath in agentReader.DetectManifests(historyStopPoint?.LocalPath!).WithCancellation(cancellationToken))
            {
                manifestsFound = true;
                await eventClient.Fire(
                    new ManifestDetectedEvent(AnalysisId, Parent, AgentExecutablePath, manifestPath),
                    cancellationToken);
            }

            if (!manifestsFound)
            {
                await eventClient.Fire(new NoManifestsDetectedEvent(AnalysisId, Parent), cancellationToken);
            }
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(Parent, error), cancellationToken);
        }
    }
}
