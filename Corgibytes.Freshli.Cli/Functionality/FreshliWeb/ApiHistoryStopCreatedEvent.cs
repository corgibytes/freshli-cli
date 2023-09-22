using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ApiHistoryStopCreatedEvent : ApplicationEventBase
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public required CachedHistoryStopPoint HistoryStopPoint { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken) =>
        await eventClient.Dispatch(
            new CheckoutHistoryActivity { HistoryStopPoint = HistoryStopPoint },
            cancellationToken);
}
