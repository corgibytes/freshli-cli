using System;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;
using TextTableFormatter;
using Environment = System.Environment;
using System.CommandLine;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsCommandRunner : CommandRunner<AgentsCommand, EmptyCommandOptions>
{
    public AgentsCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {
    }

    public override int Run(EmptyCommandOptions options, InvocationContext context) => 0;
}

public class AgentsDetectCommandRunner : CommandRunner<AgentsDetectCommand, EmptyCommandOptions>
{
    public AgentsDetectCommandRunner(IServiceProvider serviceProvider, Runner runner, AgentsDetector agentsDetector)
        : base(serviceProvider, runner) =>
        AgentsDetector = agentsDetector;

    private AgentsDetector AgentsDetector { get; }

    public override int Run(EmptyCommandOptions options, InvocationContext context)
    {
        var agents = AgentsDetector.Detect();

        var agentsAndLocations = agents.ToDictionary(Path.GetFileName);

        var basicTable = new TextTable(2);
        basicTable.AddCell("Agent file");
        basicTable.AddCell("Agent path");

        foreach (var agentAndLocation in agentsAndLocations)
        {
            basicTable.AddCell(agentAndLocation.Key);
            basicTable.AddCell(agentAndLocation.Value);
        }

        Console.WriteLine(agentsAndLocations.Count == 0
            ? CliOutput.AgentsDetectCommandRunner_Run_No_detected_agents_found
            : basicTable.Render());

        return 0;
    }
}

public class AgentsVerifyCommandRunner : CommandRunner<AgentsVerifyCommand, AgentsVerifyCommandOptions>
{
    public AgentsVerifier AgentsVerifier { get; }
    public AgentsVerifyCommandRunner(IServiceProvider serviceProvider, Runner runner, AgentsVerifier agentsVerifier)
        : base(serviceProvider, runner)
    {
        AgentsVerifier = agentsVerifier;
    }

    public override int Run(AgentsVerifyCommandOptions options, InvocationContext context)
    {
        var agents = AgentsVerifier.Verify();
        System.Console.WriteLine(AgentsVerifier + " Dona dona verify");
        Argument<string> languageNames = new Argument<string>("language-name");
        Argument<string> languageName = new("language-name", "Name of the language")
        {
            Arity = ArgumentArity.OneOrMore
        };

        foreach (string langName in Environment.GetCommandLineArgs()) 
        {
            Console.WriteLine("donas lang " + langName);
        }
        System.Console.WriteLine ("dona finds " + context.ParseResult.FindResultFor(languageName));
        

        System.Console.WriteLine(Environment.GetCommandLineArgs().ToString() + " Dona dona context");
        System.Console.WriteLine(context.ParseResult.GetValueForArgument(languageName) + " Dona dona context");

        return 0;
    }
}


