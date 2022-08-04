using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Test.Commands;

public class MockAgentsReader : IAgentReader
{
    public List<Package> RetrieveReleaseHistory(string agentExecutable, PackageURL packageUrl) => agentExecutable switch
    {
        "/usr/local/bin/freshli-agent-csharp" => packageUrl.Name switch
        {
            "flyswatter" => new List<Package>
            {
                new(new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
                    new DateTimeOffset(1990, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                new(new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.2.0"),
                    new DateTimeOffset(2001, 3, 14, 0, 0, 0, TimeSpan.Zero)),
                new(new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.3.0"),
                    new DateTimeOffset(2020, 8, 21, 0, 0, 0, TimeSpan.Zero))
            },
            _ => new List<Package>()
        },
        "/usr/local/bin/freshli-agent-javascript" => new List<Package>(),
        "/usr/local/agents/bin/freshli-agent-csharp" => new List<Package>(),
        "/home/freshli-user/bin/agents/freshli-agent-ruby" => packageUrl.Name switch
        {
            "no_release_date" => new List<Package>
            {
                new(new PackageURL("pkg:ruby/org.corgibytes.no_release_date/no_release_date@2.3.0"),
                    new DateTimeOffset(1990, 1, 29, 0, 0, 0, TimeSpan.Zero))
            },
            _ => new List<Package>()
        },
        _ => new List<Package>()
    };
}
