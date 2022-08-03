using System.IO;
using Corgibytes.Freshli.Cli.DataModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Corgibytes.Freshli.Cli.CommandOptions;

public abstract class CommandOptions
{
    public string CacheDir { get; set; } = CacheContext.DefaultCacheDir;
}
