using System.IO;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Configuration : IConfiguration
{
    public Configuration(IEnvironment environment)
    {
        GitPath = "git";
        CacheDir = Path.Combine(environment.HomeDirectory, ".freshli");
    }

    [JsonConstructor]
    public Configuration(string gitPath, string cacheDir)
    {
        GitPath = gitPath;
        CacheDir = cacheDir;
    }

    public string GitPath { get; set; }
    public string CacheDir { get; set; }
}
