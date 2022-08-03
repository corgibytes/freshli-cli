using System;
using System.Collections.Generic;
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
        "/usr/local/bin/freshli-agent-javascript" => new List<Package>(),
        "/usr/local/agents/bin/freshli-agent-csharp" => new List<Package>(),
        "/home/freshli-user/bin/agents/freshli-agent-ruby" => new List<Package>(),
        _ => new List<Package>()
    };

    private static List<Package> ReturnListOfPackages(PackageURL packageUrl) =>
        packageUrl.Name switch
        {
            "flyswatter" => new List<Package>
            {
                new(new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
                    new DateTimeOffset(1990, 11, 29, 0, 0, 0, TimeSpan.Zero))
            },
            "calculatron" => new List<Package>
            {
                new(new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@21.3"),
                    new DateTimeOffset(2022, 10, 16, 0, 0, 0, TimeSpan.Zero)),
                new(new PackageURL("pkg:nuget/org.corgibytes.calculatron/calculatron@14.6"),
                    new DateTimeOffset(2019, 12, 31, 0, 0, 0, TimeSpan.Zero))
            },
            _ => new List<Package>()
        };
}
