using Autofac;
using Corgibytes.Freshli.Cli.Formatters;
using Corgibytes.Freshli.Cli.IoC.Modules;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Corgibytes.Freshli.Cli.Runners;
using Freshli;
using System.Collections.Generic;


namespace Corgibytes.Freshli.Cli.IoC
{
    public class FreshliContainerBuilder
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new MiddlewareModule());
            builder.RegisterModule(new BaseCommandModule());
            builder.RegisterModule(new AuthCommandModule());
            builder.RegisterModule(new ScanCommandModule());

            return builder.Build();
        }
    }
}
