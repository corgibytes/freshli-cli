namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AccountNotSetUpEvent : FailureEvent
{
    public AccountNotSetUpEvent(string uiUrl)
    {
        ErrorMessage = $"Account is not setup. Please log-in to {uiUrl} and finish setting up your account.";
    }
}
