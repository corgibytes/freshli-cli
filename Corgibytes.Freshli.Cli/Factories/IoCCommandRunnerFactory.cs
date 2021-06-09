using Autofac;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.IoC;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Runners;
using Freshli;
using System.Collections.Generic;
using System.Linq;

namespace Corgibytes.Freshli.Cli.Factories
{
    public class IoCCommandRunnerFactory : ICommandRunnerFactory
    {
        private static IContainer Container { get; set; }

        static IoCCommandRunnerFactory()
        {
            Container = FreshliContainerBuilder.Build();
        }

        public ICommandRunner<AuthOptions> CreateAuthRunner( AuthOptions options )
        {
            using var scope = Container.BeginLifetimeScope();
            ICommandRunner<AuthOptions> authCommandRunner = scope.Resolve<ICommandRunner<AuthOptions>>();
            return authCommandRunner;
        }

        public ICommandRunner<ScanOptions> CreateScanRunner( ScanOptions options )
        {
            using var scope = Container.BeginLifetimeScope();

            this.InstantiateBasenOptions(options, scope, out IList<IOutputStrategy> requestedOutputStrategies, out IOutputFormatter requestedFormatter);
            var createScanCommandRunner = scope.Resolve<ScanCommandRunner.Factory>();
            return createScanCommandRunner(requestedOutputStrategies, requestedFormatter) ;
        }

        private void InstantiateBasenOptions( Option options, ILifetimeScope scope, out IList<IOutputStrategy> requestedOutputStrategies, out IOutputFormatter requestedFormatter )
        {
            // Instantiate Output Strategies and formatter based on requested output and formatter types.
            requestedOutputStrategies = options.Output.Select(output => scope.ResolveKeyed<IOutputStrategy>(output)).ToList<IOutputStrategy>();
            requestedFormatter = scope.ResolveKeyed<IOutputFormatter>(options.Format);
        }
    }
}
