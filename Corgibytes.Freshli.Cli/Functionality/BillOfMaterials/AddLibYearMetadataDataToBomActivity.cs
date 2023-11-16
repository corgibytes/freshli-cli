using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class AddLibYearMetadataDataToBomActivity : ApplicationActivityBase, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string PathToBom { get; init; }
    public required string AgentExecutablePath { get; init; }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var manifest = Parent?.Manifest;
            _ = manifest ?? throw new Exception("Parent's Manifest is null");

            var bomProcessor = eventClient.ServiceProvider.GetRequiredService<IBillOfMaterialsProcessor>();
            await bomProcessor.AddLibYearMetadataDataToBom(manifest, AgentExecutablePath, PathToBom, cancellationToken);

            await eventClient.Fire(
                new LibYearMetadataAddedToBomEvent
                {
                    Parent = this,
                    PathToBom = PathToBom,
                    AgentExecutablePath = AgentExecutablePath
                },
                cancellationToken
            );
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
        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name} - AgentExecutablePath = {AgentExecutablePath}, Manifest = {manifestId}, PathToBom = {PathToBom}";
    }
}
