using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Commands;

public interface IAgentsDetector
{
    public IList<string> Detect();
}
