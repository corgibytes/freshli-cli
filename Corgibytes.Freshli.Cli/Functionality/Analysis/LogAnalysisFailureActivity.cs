using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class LogAnalysisFailureActivity : IApplicationActivity
{
    public readonly ErrorEvent ErrorEvent;

    public LogAnalysisFailureActivity(ErrorEvent errorEvent) => ErrorEvent = errorEvent;

    public async ValueTask Handle(IApplicationEventEngine eventClient)
    {
        var logger = eventClient.ServiceProvider.GetRequiredService<ILogger<LogAnalysisFailureActivity>>();

        if (ErrorEvent is UnhandledExceptionEvent exceptionEvent)
        {
            logger.LogError(exceptionEvent.Error, "Unhandled Exception");
        }
        else
        {
            logger.LogError("{ErrorMessage}", ErrorEvent.ErrorMessage);
        }

        await eventClient.Fire(new AnalysisFailureLoggedEvent(ErrorEvent));
    }
}
