using Autofac;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.Runners;
using Freshli;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class ScanCommandModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<Runner>().As<Runner>();
            builder.RegisterType<ScanCommandRunner>();
        }
    }
}
