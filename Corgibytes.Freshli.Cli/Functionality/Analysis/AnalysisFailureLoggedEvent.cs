using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisFailureLoggedEvent : IApplicationEvent
{
    public ErrorEvent ErrorEvent { get; }

    public AnalysisFailureLoggedEvent(ErrorEvent errorEvent)
    {
        ErrorEvent = errorEvent;
    }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}

