using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Test.Commands;

public class MockAgentsReader : IAgentReader
{
    public List<Package> ListValidPackageUrls(string agentExecutable, PackageURL packageUrl) => agentExecutable switch
    {
        "/usr/local/bin/freshli-agent-csharp" => packageUrl.Name switch
        {
            "flyswatter" => new()
            {
                new(new("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"), new(1990, 1, 29, 0, 0, 0, TimeSpan.Zero)),
                new(new("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.2.0"), new(2001, 3, 14, 0, 0, 0, TimeSpan.Zero)),
                new(new("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.3.0"), new(2020, 8, 21, 0, 0, 0, TimeSpan.Zero))
            },
            _ => new()
        },
        "/usr/local/bin/freshli-agent-javascript" => new(),
        "/usr/local/agents/bin/freshli-agent-csharp" => new(),
        "/home/freshli-user/bin/agents/freshli-agent-ruby" => packageUrl.Name switch
        {
            "no_release_date" => new()
            {
                new(new("pkg:ruby/org.corgibytes.no_release_date/no_release_date@2.3.0"), new(1990, 1, 29, 0, 0, 0, TimeSpan.Zero))
            },
            _ => new()
        },
        _ => new()
    };
}

