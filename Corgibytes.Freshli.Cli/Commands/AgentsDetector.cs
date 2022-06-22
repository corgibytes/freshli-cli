
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Functionality;
using ServiceStack.Text;

namespace Corgibytes.Freshli.Cli.CommandRunners
{
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
            IList<string> filesResults = null;

            foreach (string path in paths)
            {
                var searchPath = path;
                if (path.Contains("~/"))
                {
                    string homePath = Environment.HomeDirectory;
                    homePath += Path.DirectorySeparatorChar;
                    searchPath = path.Replace("~/", homePath);
                    filesResults = Environment.GetListOfFiles(searchPath);
                }
                else
                {
                    filesResults = Environment.GetListOfFiles(searchPath);
                }

                foreach (string file in filesResults)
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
}
