using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test;

public class Startup
{
    public virtual void ConfigureServices(IServiceCollection services) =>
        new FreshliServiceBuilder(services).Register();
}
