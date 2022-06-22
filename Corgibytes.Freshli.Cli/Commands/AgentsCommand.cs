using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.CommandRunners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.OutputStrategies;


namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsCommand : Command
{
    public AgentsCommand() : base("agents", "Detects all of the language agents that are available for use")
    {
        AgentsDetectCommand detect = new();
        AddCommand(detect);
    }
}

public class AgentsDetectCommand : Command
{
    public AgentsDetectCommand() : base("detect",
        "Outputs the detected language name and the path to the language agent binary in a tabular format")
    {
        Handler = CommandHandler.Create<IHost, InvocationContext, EmptyCommandOptions>(Run);
    }

    private int Run(IHost host, InvocationContext context, EmptyCommandOptions options)
    {
        using IServiceScope scope = host.Services.CreateScope();
        var runner =
            scope.ServiceProvider.GetRequiredService<ICommandRunner<AgentsDetectCommand, EmptyCommandOptions>>();
        return runner.Run(options, context);
    }
}
