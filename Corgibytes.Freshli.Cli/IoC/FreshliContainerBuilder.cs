using Autofac;
using Corgibytes.Freshli.Cli.IoC.Modules;

namespace Corgibytes.Freshli.Cli.IoC
{
    public class FreshliContainerBuilder
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new BaseCommandModule());
            builder.RegisterModule(new AuthCommandModule());
            builder.RegisterModule(new ScanCommandModule());

            builder.RegisterModule(new InterceptorsModule());

            return builder.Build();
        }
    }
}
