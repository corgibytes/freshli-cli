using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Auth;

public class NotAuthenticatedEvent : FailureEvent
{
    public NotAuthenticatedEvent()
    {
        ErrorMessage = "Failed to verify authentication credentials. Please try logging in using the `auth` command.";
    }
}
