namespace Corgibytes.Freshli.Cli.CommandOptions;

public class CheckoutHistoryCommandOptions : CommandOptions
{
    // Arguments
    public string RepositoryId { get; set; }
    public string Sha { get; set; }

    // Options
    public string GitBinary { get; set; }
}

