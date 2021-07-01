using Autofac;
using Autofac.Extras.DynamicProxy;
using Corgibytes.Freshli.Cli.IoC.Interceptors;
using Corgibytes.Freshli.Cli.Runners;
using Freshli;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class ScanCommandModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<Runner>()
                .AsSelf();                

            builder.RegisterType<ScanCommandRunner>()
                .EnableClassInterceptors();
        }
    }
}
