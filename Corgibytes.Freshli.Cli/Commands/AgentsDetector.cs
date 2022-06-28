using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Commands;

// NOTE: This implementation should not be used like this
// This serves as a placeholder until this PR is done & merged:
// https://github.com/corgibytes/freshli-cli/pull/96
public class AgentsDetector : IAgentsDetector
{
    public IList<string> Detect()
    {
        return new List<string>()
        {
            ""
        };
    }
}

