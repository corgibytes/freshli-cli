using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Configuration : IConfiguration
{
    public Configuration(IEnvironment environment)
    {
        GitPath = "git";
        CacheDir = Path.Combine(environment.HomeDirectory, ".freshli");
    }

    public string GitPath { get; set; }
    public string CacheDir { get; set; }
}
