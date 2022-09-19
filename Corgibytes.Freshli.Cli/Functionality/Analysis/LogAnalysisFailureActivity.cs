using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class LogAnalysisFailureActivity : IApplicationActivity
{
    public readonly ErrorEvent ErrorEvent;

    public LogAnalysisFailureActivity(ErrorEvent errorEvent)
    {
        ErrorEvent = errorEvent;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        eventClient.Fire(new AnalysisFailureLoggedEvent(ErrorEvent));
    }
}
