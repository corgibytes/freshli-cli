using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsDetector : IAgentsDetector
{
    public AgentsDetector(IEnvironment environment) => Environment = environment;

    private IEnvironment Environment { get; }

    public IList<string> Detect()
    {
        var paths = Environment.DirectoriesInSearchPath;
        var agents = new Dictionary<string, string>();
        foreach (var path in paths)
        {
            var searchPath = path;
            IList<string?> filesResults;
            if (path.Contains("~/"))
            {
                var homePath = Environment.HomeDirectory;
                homePath += Path.DirectorySeparatorChar;
                searchPath = path.Replace("~/", homePath);
                filesResults = Environment.GetListOfFiles(searchPath);
            }
            else
            {
                filesResults = Environment.GetListOfFiles(searchPath);
            }

            foreach (var file in filesResults)
            {
                if (file != null && Path.GetFileName(file).StartsWith("freshli-agent-"))
                {
                    if (!agents.ContainsKey(file))
                    {
                        agents.Add(file, Path.Combine(searchPath, file));
                    }
                }
            }
        }

        return agents.Values.ToList();
    }
}
