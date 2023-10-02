using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Corgibytes.Freshli.Cli.Functionality.LibYear;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsGeneratedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string PathToBillOfMaterials { get; init; }
    public required string AgentExecutablePath { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(
                new DeterminePackagesFromBomActivity
                {
                    Parent = this,
                    PathToBom = PathToBillOfMaterials,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken
            );
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(this, error),
                cancellationToken
            );
        }
    }

    public override string ToString()
    {
        var historyStopPointId = Parent?.HistoryStopPoint?.Id ?? 0;

        var manifestId = Parent?.Manifest?.Id ?? 0;
        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name} - AgentExecutablePath = {AgentExecutablePath}, Manifest = {manifestId}, PathToBillOfMaterials = {PathToBillOfMaterials}";
    }
}
