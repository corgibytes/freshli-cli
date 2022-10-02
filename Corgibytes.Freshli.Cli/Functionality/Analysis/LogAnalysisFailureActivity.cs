using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class LogAnalysisFailureActivity : IApplicationActivity
{
    public readonly ErrorEvent ErrorEvent;

    public LogAnalysisFailureActivity(ErrorEvent errorEvent) => ErrorEvent = errorEvent;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var logger = eventClient.ServiceProvider.GetRequiredService<ILogger<LogAnalysisFailureActivity>>();

        if (ErrorEvent is UnhandledExceptionEvent)
        {
            var exceptionEvent = (UnhandledExceptionEvent) ErrorEvent;
            logger.LogError(exceptionEvent.Error, "Unhandled Exception");
        }
        else
        {
            logger.LogError(ErrorEvent.ErrorMessage);
        }

        eventClient.Fire(new AnalysisFailureLoggedEvent(ErrorEvent));
    }
}
