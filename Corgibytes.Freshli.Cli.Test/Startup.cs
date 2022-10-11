using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Test;

// ReSharper disable once UnusedType.Global
public class Startup
{
    // ReSharper disable once UnusedMember.Global
    public void ConfigureHost(IHostBuilder hostBuilder) =>
        hostBuilder
            .UseDefaultServiceProvider((_, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureServices((_, services) =>
                new FreshliServiceBuilder(services, new Configuration(new Environment())).Register());
}
