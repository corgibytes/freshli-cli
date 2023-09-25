using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class AddLibYearMetadataDataToBomActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string PathToBom { get; init; }
    public required string AgentExecutablePath { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            var bomProcessor = eventClient.ServiceProvider.GetRequiredService<IBillOfMaterialsProcessor>();
            await bomProcessor.AddLibYearMetadataDataToBom(AgentExecutablePath, PathToBom, cancellationToken);

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
}
