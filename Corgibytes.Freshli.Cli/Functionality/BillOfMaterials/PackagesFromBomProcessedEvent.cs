using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class PackagesFromBomProcessedEvent : IApplicationEvent, IHistoryStopPointProcessingTask
{
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string PathToBom { get; init; }
    public required string AgentExecutablePath { get; init; }

    public async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        try
        {
            await eventClient.Dispatch(
                new AddLibYearMetadataDataToBomActivity
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
            await eventClient.Dispatch(
                new FireHistoryStopPointProcessingErrorActivity(this, error),
                cancellationToken
            );
        }
    }
}
