using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class DestroyCacheActivity : IApplicationActivity
{
    public DestroyCacheActivity(CacheDestroyCommandOptions options, InvocationContext context)
    {
        cmdOptions = options;
        cmdContext = context;
    }

    private CacheDestroyCommandOptions cmdOptions { get; }
    private InvocationContext cmdContext { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var strConfirmDestroy = string.Format(
            CliOutput.CacheDestroyCommandRunner_Run_Prompt,
            cmdOptions.CacheDir.FullName);

        // Unless the --force flag is passed, prompt the user whether they want
        // to destroy the cache
        if (!cmdOptions.Force && !Confirm(strConfirmDestroy, cmdContext))
        {
            cmdContext.Console.Out.WriteLine(
                CliOutput.CacheDestroyCommandRunner_Run_Abort);
            return true.ToExitCode();
        }

        var strDestroyingCache = string.Format(
            CliOutput.CacheDestroyCommandRunner_Run_Destroying,
            cmdOptions.CacheDir);

        // Destroy the cache
        cmdContext.Console.Out.WriteLine(strDestroyingCache);
        try
        {
            Functionality.Cache.Destroy(cmdOptions.CacheDir).ToExitCode();
        }
        catch (CacheException error)
        {
            cmdContext.Console.Error.WriteLine(error.Message);
            return error.IsWarning.ToExitCode();
        }

        eventClient.Fire(new CacheDestroyedEvent());
    }
}
