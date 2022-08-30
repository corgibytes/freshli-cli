using System.IO;

namespace Corgibytes.Freshli.Cli.CommandOptions;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class ComputeLibYearCommandOptions : CommandOptions
{
    public FileInfo FilePath { get; set; } = null!;
}
