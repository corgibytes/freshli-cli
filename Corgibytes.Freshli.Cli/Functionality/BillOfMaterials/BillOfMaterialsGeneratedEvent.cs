using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public BillOfMaterialsGeneratedEvent(Guid analysisId, IHistoryStopPointProcessingTask parent,
        string pathToBillOfMaterials, string agentExecutablePath)
    {
        AnalysisId = analysisId;
        Parent = parent;
        PathToBillOfMaterials = pathToBillOfMaterials;
        AgentExecutablePath = agentExecutablePath;
    }

    public Guid AnalysisId { get; }
    public IHistoryStopPointProcessingTask Parent { get; }
    public string PathToBillOfMaterials { get; }
    public string AgentExecutablePath { get; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(
                new DeterminePackagesFromBomActivity(
                    AnalysisId,
                    Parent,
                    PathToBillOfMaterials,
                    AgentExecutablePath
                ),
                cancellationToken);
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(Parent, error),
                cancellationToken);
        }
    }
}
