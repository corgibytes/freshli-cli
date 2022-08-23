using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Commands;

public class AgentsDetector : IAgentsDetector
{
    [JsonProperty] private readonly IEnvironment _environment;

    public AgentsDetector(IEnvironment environment) => _environment = environment;

    public IList<string> Detect()
    {
        var paths = _environment.DirectoriesInSearchPath;
        var agents = new Dictionary<string, string>();
        foreach (var path in paths)
        {
            var searchPath = path;
            IList<string?> filesResults;
            if (path.Contains("~/"))
            {
                var homePath = _environment.HomeDirectory;
                homePath += Path.DirectorySeparatorChar;
                searchPath = path.Replace("~/", homePath);
                filesResults = _environment.GetListOfFiles(searchPath);
            }
            else
            {
                filesResults = _environment.GetListOfFiles(searchPath);
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
