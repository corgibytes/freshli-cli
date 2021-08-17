
using System;
using Corgibytes.Freshli.Cli.IoC.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.IoC
{
    public class FreshliServiceBuilder
    {
        public static void Register(IServiceCollection services)
        {            
            new BaseCommandServiceBuilder().Register(services);
            new AuthCommandServiceBuilder().Register(services);
            new ScanCommandServiceBuilder().Register(services);
        }
    }
}
