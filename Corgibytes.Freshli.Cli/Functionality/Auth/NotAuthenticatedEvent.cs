using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Auth;

public class NotAuthenticatedEvent : FailureEvent
{
    public NotAuthenticatedEvent()
    {
        ErrorMessage = "Failed to verify authentication credentials. Please try logging in using the `auth` command.";
    }

    public override ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        // Prevent logging this message as a failure.
        return ValueTask.CompletedTask;
    }
}
