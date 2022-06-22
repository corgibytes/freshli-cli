
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using ServiceStack.Text;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsDetector
{
    public IEnvironment Environment
    {
        get;
    }
    public AgentsDetector(IEnvironment environment)
    {
        Environment = environment;
    }
    public IList<string> Detect()
    {
        var paths = Environment.DirectoriesInSearchPath;
        IList<string> agents = new List<string>();
        foreach (var path in paths)
        {
            var searchPath = path;
            IList<string> filesResults;
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
                if (Path.GetFileName(file).StartsWith("freshli-agent-"))
                {
                    agents.Add(Path.Combine(searchPath, file));
                }
            }
        }

        return agents;
    }
}
