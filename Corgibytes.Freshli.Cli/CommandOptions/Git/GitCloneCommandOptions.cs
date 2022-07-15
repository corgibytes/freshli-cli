// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.CommandOptions.Git;

public class GitCloneCommandOptions : CommandOptions
{
    public string GitPath { get; set; }
    public string Branch { get; set; }
    public string RepoUrl { get; set; }
}
