using System.CommandLine;
using Corgibytes.Freshli.Cli.CommandOptions;

namespace Corgibytes.Freshli.Cli.Commands.Cache
{
    public class CacheDestroyCommand : RunnableCommand<CacheDestroyCommandOptions>
    {
        public CacheDestroyCommand()
            : base("destroy", "Deletes the Freshli cache.")
        {
            Option<bool> forceOption = new("--force", "Don't prompt to confirm destruction of cache.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };

            AddOption(forceOption);
        }
    }

}
