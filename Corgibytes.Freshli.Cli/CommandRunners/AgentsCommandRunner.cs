using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Agents;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;
using TextTableFormatter;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsCommandRunner : CommandRunner<AgentsCommand, EmptyCommandOptions>
{
    public AgentsCommandRunner(IServiceProvider serviceProvider, IRunner runner)
        : base(serviceProvider, runner)
    {
    }

    public override ValueTask<int> Run(EmptyCommandOptions options, IConsole console) => ValueTask.FromResult(0);
}

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

    public override async ValueTask<int> Run(EmptyCommandOptions options, IConsole console)
    {
        await ActivityEngine.Dispatch(new DetectAgentsActivity(AgentsDetector));

        EventEngine.On<AgentsDetectedEvent>(detectionEvent =>
        {
            FormatAndWriteToConsole(detectionEvent.AgentsAndLocations ?? new Dictionary<string, string>());
            return ValueTask.CompletedTask;
        });

        await ActivityEngine.Wait();

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

public class AgentsVerifyCommandRunner : CommandRunner<AgentsVerifyCommand, AgentsVerifyCommandOptions>
{
    private readonly IAgentsDetector _agentsDetector;

    public AgentsVerifyCommandRunner(IServiceProvider serviceProvider, IRunner runner, AgentsVerifier agentsVerifier,
        IAgentsDetector agentsDetector)
        : base(serviceProvider, runner)
    {
        _agentsDetector = agentsDetector;
        AgentsVerifier = agentsVerifier;
    }

    private AgentsVerifier AgentsVerifier { get; }

    // TODO: This method should dispatch an activity
    public override ValueTask<int> Run(AgentsVerifyCommandOptions options, IConsole console)
    {
        var agents = _agentsDetector.Detect();

        if (options.LanguageName == "")
        {
            foreach (var agentsAndPath in agents)
            {
                AgentsVerifier.RunAgentsVerify(agentsAndPath, "validating-repositories", options.CacheDir, "");
            }
        }
        else
        {
            foreach (var agentsAndPath in agents)
            {
                if (agentsAndPath.ToLower().Contains("freshli-agent-" + options.LanguageName.ToLower()))
                {
                    AgentsVerifier.RunAgentsVerify(agentsAndPath, "validating-repositories", options.CacheDir,
                        options.LanguageName);
                }
            }
        }

        return ValueTask.FromResult(0);
    }
}
