using Corgibytes.Freshli.Cli.Functionality.Support;
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
                new ServiceBuilder(services, new Configuration(new Environment())).Register()).Build();

    protected IServiceScope ServiceScope { get; } = Host.Services.CreateScope();
}
