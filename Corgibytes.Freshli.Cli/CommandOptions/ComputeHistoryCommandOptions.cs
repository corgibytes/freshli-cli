using System.IO;

namespace Corgibytes.Freshli.Cli.CommandOptions;

public class ComputeHistoryCommandOptions : CommandOptions
{
    public string RepositoryId { get; set; }
    public bool CommitHistory { get; set; }
    public string HistoryInterval { get; set; }
    public FileInfo GitPath { get; set; }
}
