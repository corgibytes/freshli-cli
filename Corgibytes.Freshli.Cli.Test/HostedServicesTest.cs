using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Test;

public abstract class HostedServicesTest
{
    protected HostedServicesTest()
    {
        ServiceScope = Host.Services.CreateScope();
    }

    private static IHost? s_host;
    protected static IHost Host
    {
        get
        {
            return s_host ??= new HostBuilder()
                .UseDefaultServiceProvider((_, options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                })
                .ConfigureServices((_, services) =>
                    new FreshliServiceBuilder(services, new Configuration(new Environment())).Register()).Build();
        }
    }
    protected IServiceScope ServiceScope { get; }
}
