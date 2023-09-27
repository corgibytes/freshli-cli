using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Support;

namespace Corgibytes.Freshli.Cli.Test.Functionality.History;

public class MockEnvironment : IEnvironment
{
    public IList<string> WindowsExecutableExtensions => new List<string>();
    public string PathSeparator => "/";

    public IList<string> GetListOfFiles(string directory) => directory switch
    {
        "/usr/local/bin" => new List<string>
        {
            "freshli-agent-java",
            "freshli-agent-javascript",
            "bash"
        },
        "/usr/local/agents/bin" => new List<string> { "freshli-agent-csharp" },
        "/home/freshli-user/bin/agents" => new List<string> { "freshli-agent-ruby" },
        _ => throw new ArgumentException("Unrecognized Directory")
    };

    public string? GetVariable(string variableName) => null;
    public bool HasExecutableBit(string fileName) => true;

    public IList<string> DirectoriesInSearchPath => new List<string>
    {
        "/usr/local/bin",
        "/usr/local/agents/bin",
        "~/bin/agents"
    };

    public string HomeDirectory => "/home/freshli-user";
    public bool IsWindows => false;
}
