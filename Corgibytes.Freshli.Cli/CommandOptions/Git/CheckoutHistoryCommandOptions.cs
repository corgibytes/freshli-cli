// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Corgibytes.Freshli.Cli.CommandOptions.Git;

public class CheckoutHistoryCommandOptions : CommandOptions
{
    public string RepositoryId { get; set; } = null!;
    public string Sha { get; set; } = null!;
    public string GitPath { get; set; } = null!;
}
