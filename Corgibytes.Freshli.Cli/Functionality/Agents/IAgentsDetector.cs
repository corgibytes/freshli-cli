using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public interface IAgentsDetector
{
    // TODO: Make this method return ValueTask or an async-friendly enumerable
    public IList<string> Detect();
}
