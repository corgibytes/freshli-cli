using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class DeterminePackagesFromBomActivity : ApplicationActivityBase, IHistoryStopPointProcessingTask, IDisposable
{
    public Thread? WaitingForChildrenThread { get; private set; }
    public required IHistoryStopPointProcessingTask? Parent { get; init; }
    public required string PathToBom { get; init; }
    public required string AgentExecutablePath { get; init; }

    public DeterminePackagesFromBomActivity() : base(100)
    {
    }

    public override async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        WaitingForChildrenThread = BuildWaitingForChildrenThread(eventClient, cancellationToken);
        WaitingForChildrenThread.Start();

        var historyStopPoint = Parent?.HistoryStopPoint;
        _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

        var logger = eventClient.ServiceProvider.GetService<ILogger<DeterminePackagesFromBomActivity>>();
        logger?.LogTrace("HistoryStopPoint = {HistoryStopPointId}: Starting retrieval of packageUrls from BomFile = {PathToBom}", historyStopPoint.Id, PathToBom);
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        try
        {
            var bomReader = eventClient.ServiceProvider.GetRequiredService<IBomReader>();
            var packageUrls = bomReader.AsPackageUrls(PathToBom);
            logger?.LogTrace(
                "HistoryStopPoint = {HistoryStopPointId}: Received {Count} packageUrls from BomFile = {PathToBom}",
                historyStopPoint.Id, packageUrls.Count, PathToBom);

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

        }
        catch (Exception error)
        {
            await eventClient.Fire(new HistoryStopPointProcessingFailedEvent(this, error), cancellationToken);
        }
        finally
        {
            stopWatch.Stop();
            logger?.LogTrace("HistoryStopPoint = {HistoryStopPointId}: Completed retrieval of packageUrls from BomFile = {PathToBom}. Elapsed time - {Duration}", historyStopPoint.Id, PathToBom, stopWatch.Elapsed);
        }
    }

    private Thread BuildWaitingForChildrenThread(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        return new Thread(Start);

        async void Start()
        {
            ApplicationTaskWaitToken? waitToken = null;
            try
            {
                waitToken = new ApplicationTaskWaitToken();
                await eventClient.RegisterChildWaitToken(this, waitToken, cancellationToken);

                var historyStopPoint = Parent?.HistoryStopPoint;
                _ = historyStopPoint ?? throw new Exception("Parent's HistoryStopPoint is null");

                var logger = eventClient.ServiceProvider.GetService<ILogger<DeterminePackagesFromBomActivity>>();

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                logger?.LogTrace(
                    "HistoryStopPoint = {HistoryStopPointId}: Waiting for packages to be determined from {BomPath}",
                    historyStopPoint.Id, PathToBom);
                await eventClient.Wait(this, cancellationToken, waitToken);
                stopWatch.Stop();
                logger?.LogTrace(
                    "HistoryStopPoint = {HistoryStopPointId}: Waited {WaitTime} for packages to be determined from {BomPath}",
                    historyStopPoint.Id, stopWatch.Elapsed.ToString(), PathToBom);

                await eventClient.Fire(
                    new PackagesFromBomProcessedEvent
                    {
                        Parent = this,
                        PathToBom = PathToBom,
                        AgentExecutablePath = AgentExecutablePath
                    },
                    cancellationToken
                );
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
                await eventClient.Fire(new UnhandledExceptionEvent(error), cancellationToken,
                    ApplicationTaskMode.Untracked);
            }
            finally
            {
                waitToken?.Signal();
            }
        }
    }

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

    public override string ToString()
    {
        var historyStopPointId = Parent?.HistoryStopPoint?.Id ?? 0;

        var manifestId = Parent?.Manifest?.Id ?? 0;
        return $"HistoryStopPoint = {historyStopPointId}: {GetType().Name} - AgentExecutablePath = {AgentExecutablePath}, Manifest = {manifestId}";
    }
}
