using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Factories
{
    public class IoCCommandRunnerFactory : ICommandRunnerFactory
    {
        public IServiceProvider ServiceProvider { get; set; }
    
        public IoCCommandRunnerFactory(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
    
        public ICommandRunner<AuthCommandOptions> CreateAuthCommandRunner()
        {
            using IServiceScope scope = ServiceProvider.CreateScope();
            return scope.ServiceProvider.GetService<ICommandRunner<AuthCommandOptions>>();
        }

        public ICommandRunner<ScanCommandOptions> CreateScanCommandRunner(ScanCommandOptions options)
        {
            using IServiceScope scope = ServiceProvider.CreateScope();
            this.InstantiateBasenOptions(options, scope, out IList<IOutputStrategy> requestedOutputStrategies, out IOutputFormatter requestedFormatter);            
            return ActivatorUtilities.CreateInstance<ScanCommandRunner>(scope.ServiceProvider, requestedOutputStrategies, requestedFormatter);
        }

        private void InstantiateBasenOptions( ScanCommandOptions options, IServiceScope scope, out IList<IOutputStrategy> requestedOutputStrategies, out IOutputFormatter requestedFormatter)
        {
            //Instantiate Output Strategies and formatter based on requested output and formatter types.
            requestedOutputStrategies = options.Output.Select(output => scope.ServiceProvider.GetRequiredNamedService<IOutputStrategy>(output)).ToList();
            requestedFormatter = scope.ServiceProvider.GetRequiredNamedService<IOutputFormatter>(options.Format);
        }
    }
}
