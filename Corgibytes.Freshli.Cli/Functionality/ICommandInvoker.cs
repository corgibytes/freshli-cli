namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICommandInvoker
{
    string Run(string executable, string arguments, string workingDirectory);
}
