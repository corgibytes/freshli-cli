namespace Corgibytes.Freshli.Cli.CommandOptions.Git;

public class CheckoutHistoryCommandOptions : CommandOptions
{
    public string RepositoryId { get; set; }
    public string Sha { get; set; }
    public string GitPath { get; set; } = "git";
}
