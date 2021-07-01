using Autofac.Extras.DynamicProxy;
using Corgibytes.Freshli.Cli.IoC.Interceptors;
using Corgibytes.Freshli.Cli.Options;
using System;

namespace Corgibytes.Freshli.Cli.Runners
{
    [Intercept(typeof(LoggerInterceptor))]
    public class AuthCommandRunner : ICommandRunner<AuthOptions>
    {
        public virtual int Run( AuthOptions options )
        {
            throw new NotImplementedException();
        }
    }
}
