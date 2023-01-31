using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading.Tasks;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public abstract class CommandRunner<TCommand, TCommandOptions> : ICommandRunner<TCommand, TCommandOptions>
    where TCommand : Command where TCommandOptions : CommandOptions.CommandOptions
{
    protected CommandRunner(IServiceProvider serviceProvider, IRunner runner)
    {
        Runner = runner;
        Services = serviceProvider;
    }

    protected IRunner Runner { get; }
    protected IServiceProvider Services { get; }

    public abstract ValueTask<int> Run(TCommandOptions options, IConsole console);

    protected static bool Confirm(string message, IConsole console, bool defaultYes = false)
    {
        // Prompt the user whether they want to proceed
        var prompt = defaultYes ? "[Y/n]" : "[y/N]";
        console.Out.Write($"{message} {prompt} ");
        var choice = Console.In.ReadLine();

        var yesChoices = new List<string>
        {
            "y",
            "Y"
        };
        var noChoices = new List<string>
        {
            "n",
            "N"
        };

        return defaultYes ? !noChoices.Contains(choice!) : yesChoices.Contains(choice!);
    }
}
