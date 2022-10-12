using System;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Environment = Corgibytes.Freshli.Cli.Functionality.Environment;

namespace Corgibytes.Freshli.Cli.Test;

public abstract class HostedServicesTest : IDisposable
{
    protected IHost Host { get; }
    protected IServiceScope ServiceScope { get; }

    protected HostedServicesTest()
    {
        Host = new HostBuilder()
            .UseDefaultServiceProvider((_, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureServices((_, services) =>
                new FreshliServiceBuilder(services, new Configuration(new Environment())).Register()).
            Build();

        ServiceScope = Host.Services.CreateScope();
    }

    public void Dispose() => Host.Dispose();
}
