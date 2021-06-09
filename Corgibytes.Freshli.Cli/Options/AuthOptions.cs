using CommandLine;

namespace Corgibytes.Freshli.Cli.Options
{
    [Verb("auth", HelpText = "Scan command help text.")]
    public class AuthOptions : Option
    {
        public override OptionType Type => OptionType.auth;
    }
}
