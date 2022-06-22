using System;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Lib;
using Microsoft.VisualBasic;
using TextTableFormatter;
using Environment = System.Environment;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsCommandRunner : CommandRunner<AgentsCommand, EmptyCommandOptions>
{
    public AgentsCommandRunner(IServiceProvider serviceProvider, Runner runner)
        : base(serviceProvider, runner)
    {

    }

    public override int Run(EmptyCommandOptions options, InvocationContext context)
    {
        return 0;
    }
}

public class AgentsDetectCommandRunner : CommandRunner<AgentsDetectCommand, EmptyCommandOptions>
{
    public AgentsDetector AgentsDetector { get; }
    public AgentsDetectCommandRunner(IServiceProvider serviceProvider, Runner runner, AgentsDetector agentsDetector)
        : base(serviceProvider, runner)
    {
        AgentsDetector = agentsDetector;
    }

    public override int Run(EmptyCommandOptions options, InvocationContext context)
    {
        var agents = AgentsDetector.Detect();

        var agentsAndLocations = agents.ToDictionary(agentLocation => Path.GetFileName(agentLocation));

        var basicTable = new TextTable(2);
        basicTable.SetColumnWidthRange(1, 60, 60);
        basicTable.SetColumnWidthRange(0, 60, 60);
        basicTable.AddCell("Agent file");
        basicTable.AddCell("Agent path");

        foreach (var agentAndLocation in agentsAndLocations)
        {
            basicTable.AddCell(agentAndLocation.Key);
            basicTable.AddCell(agentAndLocation.Value);

        }
        if (agentsAndLocations.Count == 0)
        {
            Console.WriteLine("No detected agents found");
        }
        else
        {
            Console.WriteLine(basicTable.Render());
        }

        return 0;
    }
}

