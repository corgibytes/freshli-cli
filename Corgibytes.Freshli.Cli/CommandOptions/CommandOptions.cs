using System.IO;

namespace Corgibytes.Freshli.Cli.CommandOptions;

public abstract class CommandOptions
{
    public DirectoryInfo CacheDir { get ; set; }
}
