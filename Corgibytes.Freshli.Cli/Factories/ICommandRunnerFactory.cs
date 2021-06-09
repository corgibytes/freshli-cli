using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.Runners;

namespace Corgibytes.Freshli.Cli.Factories
{
    public interface ICommandRunnerFactory
    {
        ICommandRunner<ScanOptions> CreateScanRunner( ScanOptions options );
        ICommandRunner<AuthOptions> CreateAuthRunner( AuthOptions options );

    }
}
