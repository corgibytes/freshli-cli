using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands
{
    public class ScanCommand : BaseCommand<ScanCommandOptions>
    {
        public ScanCommand() : base("scan", "Scan command returns metrics results for given local repository path")
        {

            Argument<string> pathArgument = new("path", "Source code repository path")
            {
                Arity = ArgumentArity.ExactlyOne
            };

            AddArgument(pathArgument);            
        }
    }
}
