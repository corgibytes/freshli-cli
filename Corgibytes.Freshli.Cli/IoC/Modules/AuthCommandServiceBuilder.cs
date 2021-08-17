using System.CommandLine.Hosting;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class AuthCommandServiceBuilder
    {
        public void Register(IServiceCollection collection)
        {
            collection.AddScoped<ICommandRunner<AuthCommandOptions>, AuthCommandRunner>();
            collection.AddOptions<AuthCommandOptions>().BindCommandLine();
        }
    }
}
