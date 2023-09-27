using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.Commands.Scan.Formatters;
using Corgibytes.Freshli.Cli.Commands.Scan.OutputStrategies;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Corgibytes.Freshli.Cli.Commands.Scan;

public class ScanCommandOptions : CommandOptions
{
    public FormatType Format { get; set; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public IList<OutputStrategyType> Output { get; set; } = new List<OutputStrategyType>();
    public DirectoryInfo Path { get; set; } = null!;
}
