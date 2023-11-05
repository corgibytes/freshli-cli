namespace Corgibytes.Freshli.Cli.Commands.Analyze;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class AnalyzeCommandOptions : CommandOptions
{
    public string GitPath { get; set; } = null!;
    public string? Branch { get; set; }
    public bool CommitHistory { get; set; }
    public string HistoryInterval { get; set; } = null!;
    public string RepositoryLocation { get; set; } = null!;
    public bool LatestOnly { get; set; }
    public string? Project { get; set; }
}
