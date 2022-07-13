using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

// This is just a mock for now and it should not be used like this.
// Once we have an actual agent we can use, then we can start processing the data from it.
public class AgentReader : IAgentReader
{
    public List<Package> ListValidPackageUrls(string agentExecutable, PackageURL packageUrl) => agentExecutable switch
    {
        "/usr/local/bin/freshli-agent-csharp" => ReturnListOfPackages(packageUrl),
        _ when agentExecutable.Contains("freshli-agent-test") => ReturnListOfPackages(packageUrl),
        "/usr/local/bin/freshli-agent-javascript" => new(),
        "/usr/local/agents/bin/freshli-agent-csharp" => new(),
        "/home/freshli-user/bin/agents/freshli-agent-ruby" => new(),
        _ => new()
    };

    private static List<Package> ReturnListOfPackages(PackageURL packageUrl) =>
    packageUrl.Name switch
    {
        "flyswatter" => new()
        {
            new(new("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
                new(1990, 11, 29, 0, 0, 0, TimeSpan.Zero))
        },
        "calculatron" => new()
        {
            new(new("pkg:nuget/org.corgibytes.calculatron/calculatron@21.3"),
                new(2022, 10, 16, 0, 0, 0, TimeSpan.Zero)),
            new(new("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6"),
                new(2019, 12, 31, 0, 0, 0, TimeSpan.Zero))
        },
        _ => new()
    };
}
