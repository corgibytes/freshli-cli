namespace Corgibytes.Freshli.Cli.CommandOptions;

public class GitCommandOptions : CommandOptions { }

public class GitCloneCommandOptions : CommandOptions
{
    public string GitPath { get; set; }
    public string Branch { get; set; }
    public string RepoUrl { get; set; }
}
