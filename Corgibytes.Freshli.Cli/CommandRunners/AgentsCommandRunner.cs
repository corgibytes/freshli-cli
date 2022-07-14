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
        context.Console.WriteLine(options.LanguageName + " Dona's lang argument");

        StringBuilder stdOutBuffer = new StringBuilder();
        var executionLocation = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
        var executable = new FileInfo(executionLocation.FullName + "/freshli");
        var command = CliWrap.Cli.Wrap(executable.FullName).WithArguments(
                args => args
                    .Add("agents")
                    .Add("detect")
                    
            )
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer));

            using var task = command.ExecuteAsync().Task;
            task.Wait();

            Console.WriteLine( " Dona dona \n" + stdOutBuffer.ToString() + "\n Dona dona123finish");

            
            
            foreach(string x in stdOutBuffer.ToString().Split('|',StringSplitOptions.None)){
                if (x.Length != 0)
                {
                    Console.WriteLine("Dona string x: 123 ");
                    Console.WriteLine("Dona string x: " + x);
                    Console.WriteLine("Dona string x.length: " + x.Length);
                }
            }

        if (options.LanguageName.Length == 0) {
            
            Console.WriteLine("length is zero");
        } else 
        {
            Console.WriteLine(options.LanguageName.Length + " my word length");
        }

        context.Console.WriteLine(options.Workers + 1 + " Dona's workers found");
        return 0;
    }
}

