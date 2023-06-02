using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;
using TextTableFormatter;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsDetectCommandRunner : CommandRunner<AgentsDetectCommand, EmptyCommandOptions>
{
    public AgentsDetectCommandRunner(IServiceProvider serviceProvider, IRunner runner, IAgentsDetector agentsDetector,
        IApplicationActivityEngine activityEngine, IApplicationEventEngine eventEngine)
        : base(serviceProvider, runner)
    {
        ActivityEngine = activityEngine;
        EventEngine = eventEngine;
        AgentsDetector = agentsDetector;
    }

    private IAgentsDetector AgentsDetector { get; }
    private IApplicationActivityEngine ActivityEngine { get; }
    private IApplicationEventEngine EventEngine { get; }

    public override async ValueTask<int> Run(EmptyCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        var activity = new DetectAgentsActivity(AgentsDetector);

        EventEngine.On<AgentsDetectedEvent>(detectionEvent =>
        {
            FormatAndWriteToConsole(detectionEvent.AgentsAndLocations ?? new Dictionary<string, string>());
            return ValueTask.CompletedTask;
        });

        await ActivityEngine.Dispatch(activity, cancellationToken);
        await ActivityEngine.Wait(activity, cancellationToken);

        return 0;
    }

    private static void FormatAndWriteToConsole(Dictionary<string, string> agentsAndLocations)
    {
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
    }
}
