using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

/**
 *  A failure is any failure we can't recover from during the analysis, blocking the analysis from continuing.
 */
public abstract class FailureEvent : ErrorEvent
{
    public override void Handle(IApplicationActivityEngine eventClient)
    {
        eventClient.Dispatch(new LogAnalysisFailureActivity(this));
    }
}
