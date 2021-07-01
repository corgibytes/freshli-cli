using Autofac;
using Corgibytes.Freshli.Cli.IoC.Interceptors;
using System;

namespace Corgibytes.Freshli.Cli.IoC.Modules
{
    public class InterceptorsModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.Register(c => new LoggerInterceptor(Console.Out));
        }
    }
}
