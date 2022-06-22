using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class MockEnvironment : IEnvironment
{
    public IList<string> GetListOfFiles(string directory) => directory switch
    {
        "/usr/local/bin" => new List<string>()
        {
            "freshli-agent-java", "freshli-agent-javascript", "bash"
        },
        "/usr/local/agents/bin" => new List<string>()
        {
            "freshli-agent-csharp"
        },
        "/home/freshli-user/bin/agents" => new List<string>()
        {
            "freshli-agent-ruby"
        },
        _ => throw new ArgumentException("Unrecognized Directory")
    };

    public IList<string> DirectoriesInSearchPath
    {
        get
        {
            return new List<string>() { "/usr/local/bin", "/usr/local/agents/bin", "~/bin/agents" };
        }
    }

    public string HomeDirectory
    {
        get
        {
            return "/home/freshli-user";
        }
    }
}
