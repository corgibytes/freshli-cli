// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Corgibytes.Freshli.Cli.Commands;

public abstract class CommandOptions
{
    public string CacheDir { get; init; } = null!;
}
