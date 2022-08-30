// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.CommandOptions;

public class ComputeHistoryCommandOptions : CommandOptions
{
    public IAnalysisLocation AnalysisLocation { get; set; } = null!;
    public string HistoryInterval { get; set; } = null!;
    public string GitPath { get; set; } = null!;
}
