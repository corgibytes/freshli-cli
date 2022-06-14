using System;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public abstract class CommandRunner<T> : ICommandRunner<T> where T : CommandOptions.CommandOptions
{
    protected Runner Runner { get; }
    protected IServiceProvider Services { get; }

    public CommandRunner(IServiceProvider serviceProvider, Runner runner)
    {
        Runner = runner;
        Services = serviceProvider;
    }

    public abstract int Run(T options);

}
