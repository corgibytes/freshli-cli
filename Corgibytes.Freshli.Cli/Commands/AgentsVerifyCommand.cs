using System;
using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsVerifyCommand : RunnableCommand<AgentsVerifyCommand, AgentsVerifyCommandOptions>
{
    public AgentsVerifyCommand() : base("verify",
        "Used to test all of the language agents that are available for use.")
    {
        Argument<string> languageName = new("language-name", "Name of the language")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        AddArgument(languageName);

        Option<int> formatOption = new(new[] { "--workers" },
            description:
            "Represents the number of worker processes that should be running at any given time. This defaults to twice the number of CPU cores.",
            getDefaultValue: () => Environment.ProcessorCount + 1)
        {
            AllowMultipleArgumentsPerToken = false,
            Arity = ArgumentArity.ExactlyOne
        };
        AddOption(formatOption);
    }
}
