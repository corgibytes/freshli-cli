using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.CommandRunners
{
    public interface ICommandRunner<T> where T : ICommandOption
    {
        public int Run( T options );
    }
}
