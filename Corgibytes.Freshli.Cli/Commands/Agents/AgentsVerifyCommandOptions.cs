namespace Corgibytes.Freshli.Cli.Commands.Agents;

public class AgentsVerifyCommandOptions : CommandOptions
{
    public AgentsVerifyCommandOptions(string languageName) => LanguageName = languageName;

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string LanguageName { get; }
}
