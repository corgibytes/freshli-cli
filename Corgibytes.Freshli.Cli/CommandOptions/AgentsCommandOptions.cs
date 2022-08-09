namespace Corgibytes.Freshli.Cli.CommandOptions;

public class AgentsVerifyCommandOptions : CommandOptions
{
    public AgentsVerifyCommandOptions(string languageName)
    {
        LanguageName = languageName;
    }
    public string LanguageName { get; }
}
