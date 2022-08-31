using System.IO;

namespace Corgibytes.Freshli.Cli.CommandOptions;

public class AnalyzeCommandOptions : CommandOptions
{
    public FileInfo GitPath { get; set; } = null!;
    // public string Branch { get; set; } = null!;
    // public bool CommitHistory { get; set; }
    // public string HistoryInterval { get; set; } = null!;
    // public int? Workers { get; set; } = null!;
}
