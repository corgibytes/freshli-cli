namespace Corgibytes.Freshli.Cli.Functionality;

public interface IInvoke
{
    string Command(string executable, string arguments, string workingDirectory);
}
