using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class ManifestDetectedEvent : ApplicationEventBase, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required CachedManifest? Manifest { get; init; }
    public required string AgentExecutablePath { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(
                new GenerateBillOfMaterialsActivity
                {
                    AgentExecutablePath = AgentExecutablePath,
                    Parent = this
                },
                cancellationToken
            );
        }
        catch (Exception error)
        {
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(this, error),
                cancellationToken);
        }
    }

    public override string ToString()
    {
        var historyStopPoint = Parent?.HistoryStopPoint;
        var historyStopPointId = Parent?.HistoryStopPoint?.Id ?? 0;

        var manifestId = Manifest?.Id ?? 0;
        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name} - AgentExecutablePath = {AgentExecutablePath}, Manifest = {manifestId}";
    }
}
