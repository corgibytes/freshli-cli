using System;
using Castle.Core.Configuration;
using Corgibytes.Freshli.Cli.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Corgibytes.Freshli.Cli.Test
{
    public class Startup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
            new FreshliServiceBuilder(services).Register();
        }

    }
}
