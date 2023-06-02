using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.History;

public class FireHistoryStopPointProcessingErrorActivity : IApplicationActivity, IHistoryStopPointProcessingTask
{
    public FireHistoryStopPointProcessingErrorActivity(IHistoryStopPointProcessingTask parent, Exception error)
    {
        Parent = parent;
        Error = error;
    }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(Parent, Error), cancellationToken);
    }

    public IHistoryStopPointProcessingTask Parent { get; }
    public Exception Error { get; }

    public int Priority
    {
        get
        {
            return 0;
        }
    }
}
