using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class LogAnalysisFailureActivity : IApplicationActivity
{
    public readonly ErrorEvent ErrorEvent;

    public LogAnalysisFailureActivity(ErrorEvent errorEvent) => ErrorEvent = errorEvent;

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        if (IsCancellationException(ErrorEvent))
        {
            return;
        }

        var logger = eventClient.ServiceProvider.GetRequiredService<ILogger<LogAnalysisFailureActivity>>();

        logger.LogError("{ErrorMessage}", ErrorEvent.ErrorMessage);
        if (ErrorEvent.Exception != null)
        {
            logger.LogError("{Exception}", ErrorEvent.Exception);
        }

        await eventClient.Fire(new AnalysisFailureLoggedEvent(ErrorEvent), cancellationToken);
    }

    private bool IsCancellationException(ErrorEvent errorEvent) =>
        errorEvent.Exception is TaskCanceledException or OperationCanceledException;
}
