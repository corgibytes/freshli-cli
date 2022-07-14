namespace Corgibytes.Freshli.Cli.CommandOptions;

public class AgentsCommandOptions : CommandOptions
{
}

public class AgentsDetectCommandOptions : CommandOptions
{
}

public class AgentsVerifyCommandOptions : CommandOptions
{
    public string LanguageName{ get; set; }
    public int Workers{ get; set; }
}



