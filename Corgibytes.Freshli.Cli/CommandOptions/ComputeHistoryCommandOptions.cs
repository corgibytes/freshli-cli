// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Corgibytes.Freshli.Cli.CommandOptions;

public class ComputeHistoryCommandOptions : CommandOptions
{
    public string RepositoryId { get; set; } = null!;
    public bool CommitHistory { get; set; }
    public string HistoryInterval { get; set; } = null!;
    public string GitPath { get; set; } = null!;
}
