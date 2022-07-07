using System.IO;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.CommandOptions;

public abstract class CommandOptions
{
    public DirectoryInfo CacheDir { get; set; }
}
