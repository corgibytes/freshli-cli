using System.IO;
using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Configuration : IConfiguration
{
    public string GitPath { get; set; }
    public string CacheDir { get; set; }

    public Configuration(IEnvironment environment)
    {
        GitPath = "git";
        CacheDir = Path.Combine(environment.HomeDirectory, ".freshli");
    }
}
