using System;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Resources;
using Corgibytes.Freshli.Lib;
using TextTableFormatter;
using Environment = System.Environment;
using System.CommandLine;
using System.Reflection;
using CliWrap;
using System.Diagnostics;
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

        // Path.GetFileName returns string?, but ToDictionary needs string. We resolve this with the following, and
        // then tell the compiler to stop complaining that GetFileName(x) is never null; this is about the return type.
        // ReSharper disable once ConstantNullCoalescingCondition
        var agentsAndLocations = agents.ToDictionary(
            x => Path.GetFileName(x) ?? throw new ArgumentException("No file name for given path.")
        );

        return 0;
    }
}

public class AgentsVerifyCommandRunner : CommandRunner<AgentsVerifyCommand, AgentsVerifyCommandOptions>
{
    private readonly IAgentsDetector _agentsDetector;
    public AgentsVerifier AgentsVerifier { get; }
    public AgentsVerifyCommandRunner(IServiceProvider serviceProvider, Runner runner, AgentsVerifier agentsVerifier, IAgentsDetector agentsDetector)
        : base(serviceProvider, runner)
    {
        _agentsDetector = agentsDetector;
        AgentsVerifier = agentsVerifier;
    }

    public override int Run(AgentsVerifyCommandOptions options, InvocationContext context)
    {   
        var agents = _agentsDetector.Detect();

        if (options.LanguageName == null)
        {   
            foreach (var agentsAndPath in agents)
            {
              AgentsVerifier.RunAgentsVerify(agentsAndPath,"validating-repositories", options.CacheDir, "");
            }
        } else
        {            
            foreach (var agentsAndPath in agents)
            {
                if(agentsAndPath.ToLower().Contains("freshli-agent-"+options.LanguageName.ToLower())){
                AgentsVerifier.RunAgentsVerify(agentsAndPath,"validating-repositories", options.CacheDir, options.LanguageName);
                }
                
            }
        }

        return 0;
    }
}
