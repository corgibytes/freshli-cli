using Autofac;
using Corgibytes.Freshli.Cli.Options;
using Corgibytes.Freshli.Cli.Runners;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class AuthCommandModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<AuthCommandRunner>().As<ICommandRunner<AuthOptions>>();
        }
    }
}
