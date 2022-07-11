// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.CommandOptions;

public class GitCommandOptions : CommandOptions
{
}

public class GitCloneCommandOptions : CommandOptions
{
    public string GitPath { get; set; } = null!;
    public string RepoUrl { get; set; } = null!;
    public string? Branch { get; set; }
}
