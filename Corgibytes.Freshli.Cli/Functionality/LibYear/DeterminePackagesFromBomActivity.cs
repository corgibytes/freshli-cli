using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class DeterminePackagesFromBomActivity : IApplicationActivity, IHistoryStopPointProcessingTask, IDisposable
{
    public Thread? WaitingForChildrenThread { get; private set; }

    public int Priority
    {
        get { return 100; }
    }

    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string PathToBom { get; init; }
    public required string AgentExecutablePath { get; init; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var historyStopPoint = Parent?.HistoryStopPoint;
        _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

        var logger = eventClient.ServiceProvider.GetService<ILogger<DeterminePackagesFromBomActivity>>();
        logger?.LogTrace("Handling retrieval of packageUrls from BomFile = {PathToBom}", PathToBom);

        try
        {
            var bomReader = eventClient.ServiceProvider.GetRequiredService<IBomReader>();
            var packageUrls = bomReader.AsPackageUrls(PathToBom);
            logger?.LogTrace("Received {Count}  packageUrls from BomFile = {PathToBom}",
                packageUrls.Count, PathToBom);

            foreach (var packageUrl in packageUrls)
            {
                if (packageUrl == null)
                {
                    throw new Exception($"Null package URL detected for in {PathToBom}");
                }

                await eventClient.Fire(
                    new PackageFoundEvent
                    {
                        Parent = this,
                        AgentExecutablePath = AgentExecutablePath,
                        Package = packageUrl
                    },
                    cancellationToken
                );
            }

            if (packageUrls.Count == 0)
            {
                await eventClient.Fire(new NoPackagesFoundEvent(this), cancellationToken);
            }

            WaitingForChildrenThread = BuildWaitingForChildrenThread(eventClient, cancellationToken);
            WaitingForChildrenThread.Start();
        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(this, error), cancellationToken);
        }
    }

    private Thread BuildWaitingForChildrenThread(IApplicationEventEngine eventClient, CancellationToken cancellationToken) =>
        new(() =>
        {
            try
            {
                eventClient.Wait(this, cancellationToken).AsTask().Wait(cancellationToken);
                eventClient.Fire(
                    new PackagesFromBomProcessedEvent
                    {
                        Parent = this,
                        PathToBom = PathToBom,
                        AgentExecutablePath = AgentExecutablePath
                    },
                    cancellationToken,
                    ApplicationTaskMode.Untracked
                ).AsTask().Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Don't do anything, we're just exiting
            }
            catch (ThreadInterruptedException)
            {
                // Don't do anything, the thread is being disposed of
            }
            catch (Exception error)
            {
                eventClient.Fire(
                    new UnhandledExceptionEvent(error),
                    cancellationToken,
                    ApplicationTaskMode.Untracked
                ).AsTask().Wait(cancellationToken);
            }
        });

    private const int WaitingForChildrenThreadStopTimeout = 200;
    public void Dispose()
    {
        StopWaitingForChildren();

        GC.SuppressFinalize(this);
    }

    public void StopWaitingForChildren()
    {
        if (WaitingForChildrenThread == null)
        {
            return;
        }

        var wasJoined = WaitingForChildrenThread.Join(WaitingForChildrenThreadStopTimeout);
        if (wasJoined)
        {
            return;
        }

        WaitingForChildrenThread.Interrupt();
        WaitingForChildrenThread.Join();
    }
}
