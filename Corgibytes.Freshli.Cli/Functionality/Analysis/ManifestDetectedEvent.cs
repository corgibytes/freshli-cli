using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ManifestDetectedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public ManifestDetectedEvent(Guid analysisId, IHistoryStopPointProcessingTask parent, string agentExecutablePath,
        string manifestPath)
    {
        AnalysisId = analysisId;
        Parent = parent;
        HistoryStopPointId = parent.HistoryStopPointId;
        AgentExecutablePath = agentExecutablePath;
        ManifestPath = manifestPath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopPointProcessingTask Parent { get; }
    public int HistoryStopPointId { get; }
    public string AgentExecutablePath { get; }
    public string ManifestPath { get; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(new GenerateBillOfMaterialsActivity(
                AnalysisId,
                AgentExecutablePath,
                Parent,
                ManifestPath
            ), cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(Parent, error),
                cancellationToken);
        }
    }
}
