using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Lib;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class ScanCommandServiceBuilder
    {
        public void Register(IServiceCollection collection)
        {
            collection.AddScoped<Runner>();
            collection.AddTransient<ICommandRunner<ScanCommandOptions>, ScanCommandRunner>();
            collection.AddOptions<ScanCommandOptions>().BindCommandLine();
        }
    }
}
