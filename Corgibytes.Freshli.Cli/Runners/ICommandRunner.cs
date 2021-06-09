using Corgibytes.Freshli.Cli.Options;

namespace Corgibytes.Freshli.Cli.Runners
{
    public interface ICommandRunner<T> where T : IOption
    {
        public int Run( T options );
    }
}
