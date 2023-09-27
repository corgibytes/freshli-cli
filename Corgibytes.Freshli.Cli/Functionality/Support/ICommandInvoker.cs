using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Support;

public interface ICommandInvoker
{
    ValueTask<string> Run(string executable, string arguments, string workingDirectory, int maxRetries = 0);
}
