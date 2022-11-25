using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsDetector : IAgentsDetector
{
    private readonly IExecutableFinder _executableFinder;
    private readonly IEnvironment _environment;

    public AgentsDetector(IExecutableFinder executableFinder, IEnvironment environment)
    {
        _executableFinder = executableFinder;
        _environment = environment;
    }

    public IList<string> Detect()
    {
        return _executableFinder
            .GetExecutables()
            .Where(value => value.Split(_environment.PathSeparator).Last().StartsWith("freshli-agent-"))
            .ToList();
    }
}
