using System.CommandLine.Invocation;
using System.CommandLine.IO;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Functionality.CacheDestroy;

public class DestroyCacheActivity : IApplicationActivity
{
    public DestroyCacheActivity(CacheDestroyCommandOptions options, InvocationContext context)
    {
        CmdOptions = options;
        CmdContext = context;
    }

    private CacheDestroyCommandOptions CmdOptions { get; }
    private InvocationContext CmdContext { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        // Unless the --force flag is passed, prompt the user whether they want
        // to destroy the cache
        if (!CmdOptions.Force)
        {
            eventClient.Fire(new ConfirmationRequiredEvent());
            return;
        }

        var strDestroyingCache = string.Format(
            CliOutput.CacheDestroyCommandRunner_Run_Destroying,
            CmdOptions.CacheDir);

        // Destroy the cache
        CmdContext.Console.Out.WriteLine(strDestroyingCache);
        try
        {
            var exitCode = Cache.Destroy(CmdOptions.CacheDir).ToExitCode();
            eventClient.Fire(new CacheDestroyedEvent() {ExitCode = exitCode});
        }
        catch (CacheException error)
        {
            eventClient.Fire(new CacheDestroyFailedEvent() {ResultMessage = error.Message});
        }
    }
}
