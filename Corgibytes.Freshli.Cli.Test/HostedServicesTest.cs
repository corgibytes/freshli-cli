using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Test;

public abstract class HostedServicesTest
{
    private static IHost? s_host;

    private static IHost Host =>
        s_host ??= new HostBuilder()
            .UseDefaultServiceProvider((_, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureServices((_, services) =>
                new FreshliServiceBuilder(services, new Configuration(new Environment())).Register()).Build();

    protected IServiceScope ServiceScope { get; } = Host.Services.CreateScope();
}
