using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class ForceDestroyCacheActivity : IApplicationActivity
{
    public ForceDestroyCacheActivity(CacheDestroyCommandOptions options, InvocationContext context)
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
            eventClient.Fire(new CacheDestroyedEvent(error.Message));
            return;
        }

        eventClient.Fire(new CacheDestroyedEvent());
    }
}
