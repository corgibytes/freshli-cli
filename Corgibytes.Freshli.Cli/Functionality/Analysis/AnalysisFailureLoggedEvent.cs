using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisFailureLoggedEvent : ApplicationEventBase
{
    public AnalysisFailureLoggedEvent(ErrorEvent errorEvent) => ErrorEvent = errorEvent;

    public ErrorEvent ErrorEvent { get; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
