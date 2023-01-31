using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisFailureLoggedEvent : IApplicationEvent
{
    public AnalysisFailureLoggedEvent(ErrorEvent errorEvent) => ErrorEvent = errorEvent;

    public ErrorEvent ErrorEvent { get; }

    public ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        return ValueTask.CompletedTask;
    }
}
