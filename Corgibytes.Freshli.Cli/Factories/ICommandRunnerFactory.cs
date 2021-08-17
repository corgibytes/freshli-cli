using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;

namespace Corgibytes.Freshli.Cli.Factories
{
    public interface ICommandRunnerFactory
    {
        ICommandRunner<ScanCommandOptions> CreateScanCommandRunner(ScanCommandOptions scanOptions);
        ICommandRunner<AuthCommandOptions> CreateAuthCommandRunner();
    }
}
