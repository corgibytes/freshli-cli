using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.OutputStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsCommand : Command
{
    public AgentsCommand() : base("agents", "Detects all of the language agents that are available for use")
    {
        AgentsDetectCommand detect = new();
        AddCommand(detect);
    }
}

public class AgentsDetectCommand : RunnableCommand<AgentsDetectCommand, EmptyCommandOptions>
{
    public AgentsDetectCommand() : base("detect",
        "Outputs the detected language name and the path to the language agent binary in a tabular format")
    {
    }
}
