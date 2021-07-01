using Autofac.Extras.DynamicProxy;
using Corgibytes.Freshli.Cli.IoC.Interceptors;
using Corgibytes.Freshli.Cli.Options;

namespace Corgibytes.Freshli.Cli.Runners
{

    public interface ICommandRunner<T> where T : IOption
    {
        public int Run( T options );
    }
}
