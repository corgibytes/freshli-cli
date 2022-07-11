using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Commands;

namespace Corgibytes.Freshli.Cli.Test.Commands;

public class MockAgentsDetector : IAgentsDetector
{
    public IList<string> Detect() =>
        new List<string>
        {
            "/usr/local/bin/freshli-agent-csharp",
            "/usr/local/bin/freshli-agent-javascript",
            "/usr/local/agents/bin/freshli-agent-csharp",
            "/home/freshli-user/bin/agents/freshli-agent-ruby"
        };
}
