using System.CommandLine.Invocation;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public interface ICommandRunner<in T> where T : CommandOptions.CommandOptions
{
    public int Run(T options, InvocationContext context);
}
