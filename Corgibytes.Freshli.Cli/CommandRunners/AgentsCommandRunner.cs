using System;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;
using TextTableFormatter;

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
    public AgentsDetectCommandRunner(IServiceProvider serviceProvider, Runner runner, IAgentsDetector agentsDetector)
        : base(serviceProvider, runner) =>
        AgentsDetector = agentsDetector;

    private IAgentsDetector AgentsDetector { get; }

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
