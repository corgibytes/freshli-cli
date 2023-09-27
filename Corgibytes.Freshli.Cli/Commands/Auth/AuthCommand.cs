namespace Corgibytes.Freshli.Cli.Commands.Auth;

public class AuthCommand : RunnableCommand<AuthCommand, EmptyCommandOptions>
{
    public AuthCommand() : base("auth", "Authenticates CLI operations against a Freshli account")
    {

    }
}
