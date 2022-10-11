using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Test;

// ReSharper disable once UnusedType.Global
public class Startup
{
    private static IConfiguration Configuration { get; } = new Configuration(new Environment());

    // ReSharper disable once UnusedMember.Global
    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((_, services) =>
        {
            new FreshliServiceBuilder(services, Configuration).Register();
        });
    }
}
