using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class HistoryIntervalStopFoundEvent : ApplicationEventBase
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public required CachedHistoryStopPoint HistoryStopPoint { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        var logger = eventClient.ServiceProvider.GetRequiredService<ILogger<HistoryIntervalStopFoundEvent>>();
        logger.LogDebug("Started processing history stop point {id}", HistoryStopPoint.Id);
        await eventClient.Dispatch(
            new CreateApiHistoryStopActivity { HistoryStopPoint = HistoryStopPoint },
            cancellationToken
        );
    }
}
