using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap.EventStream;
using CliWrap.Exceptions;
using Corgibytes.Freshli.Cli.Functionality;
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentManager : IAgentManager, IDisposable
{
    // TODO: Make this a configurable value
    private const int ServiceStartTimeoutInSeconds = 2;

    private readonly ICacheManager _cacheManager;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<AgentDescriptor>> _agentsByExecutable = new();

    private readonly ILogger<AgentManager> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly PortFinder _portFinder = new();

    private static readonly List<Type> s_acceptableExceptions = new()
    {
        typeof(TaskCanceledException),
        typeof(OperationCanceledException),
        typeof(CommandExecutionException)
    };

    public AgentManager(
        ICacheManager cacheManager, ILogger<AgentManager> logger,
        IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _cacheManager = cacheManager;
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    private ConcurrentQueue<AgentDescriptor> GetAgentsFor(string agentExecutablePath)
    {
        return _agentsByExecutable.GetOrAdd(agentExecutablePath, new ConcurrentQueue<AgentDescriptor>());
    }

    public IAgentReader GetReader(string agentExecutablePath, CancellationToken cancellationToken = default)
    {
        LogGettingAgentReader(agentExecutablePath);
        var agents = GetAgentsFor(agentExecutablePath);

        lock (agents)
        {
            AgentDescriptor agent;
            if (agents.Count < _configuration.AgentServiceCount)
            {
                LogStartingNewAgentService();
                var startAgentTask = StartAgentServiceOnAvailablePort(agentExecutablePath, cancellationToken);
                WithAcceptableExceptions(() => startAgentTask.Wait(cancellationToken));

                if (startAgentTask.Status != TaskStatus.RanToCompletion)
                {
                    LogFailedToStart();
                    throw new AgentRetrievalFailure();
                }

                agent = startAgentTask.Result;
                agents.Enqueue(agent);

                LogNewAgentReader(agent);
                return agent.AgentReader;
            }

            if (!agents.TryDequeue(out agent))
            {
                LogAgentQueueInUnexpectedState();
                throw new AgentRetrievalFailure();
            }

            LogRetrievingExistingAgentReader(agent);
            agents.Enqueue(agent);
            return agent.AgentReader;
        }
    }

    private async Task<AgentDescriptor> StartAgentServiceOnAvailablePort(string agentExecutablePath, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            LogAttemptingToStart();

            if (cancellationToken.IsCancellationRequested)
            {
                LogGivingUpBecauseOfCancellationRequest();
                return default;
            }

            var port = _portFinder.FindNext();

            var forcefulShutdown = new CancellationTokenSource();

            var (agentRunnerTask, isServiceListening) = AttemptToStartAgentRunner(
                agentExecutablePath, port, forcefulShutdown, cancellationToken);

            if (isServiceListening)
            {
                var agentReader = CreateAgentReader(port);
                return new AgentDescriptor(agentReader, agentRunnerTask, forcefulShutdown);
            }

            LogFailedToStart(agentExecutablePath, port);

            await ForcefullyShutdownAgentRunner(forcefulShutdown, agentRunnerTask);
        }
    }

    private async Task ForcefullyShutdownAgentRunner(CancellationTokenSource forcefulShutdown, Task agentRunnerTask)
    {
        LogForcefullyStopping();
        forcefulShutdown.Cancel();
        await WithAcceptableExceptionsAsync(async () => await agentRunnerTask);
        forcefulShutdown.Dispose();
    }

    private AgentReader CreateAgentReader(int port)
    {
        LogConnectingToGrpcService(port);
        var channel = GrpcChannel.ForAddress($"http://localhost:{port}");
        var grpcClient = new Agent.Agent.AgentClient(channel);
        var agentReader = new AgentReader(
            _cacheManager, grpcClient, _serviceProvider.GetRequiredService<ILogger<AgentReader>>()
        );
        return agentReader;
    }

    private (Task, bool) AttemptToStartAgentRunner(string agentExecutablePath, int port,
        CancellationTokenSource forcefulShutdown, CancellationToken cancellationToken)
    {
        var isServiceListening = false;

        var command = CliWrap.Cli.Wrap(agentExecutablePath).WithArguments(new List<string>
        {
            "start-server",
            port.ToString()
        });

        LogStartingAgent(agentExecutablePath, port);
        var agentRunnerTask = Task.Run(async () =>
        {
            // ReSharper disable once AccessToDisposedClosure
            var commandEvents = command.ListenAsync(
                Encoding.UTF8,
                Encoding.UTF8,
                cancellationToken,
                // ReSharper disable once AccessToDisposedClosure
                forcefulShutdown.Token
            );

            // ReSharper disable once AccessToDisposedClosure
            await foreach (var commandEvent in commandEvents.WithCancellation(forcefulShutdown.Token))
            {
                LogReceivedCommand(commandEvent);
            }
        }, cancellationToken);

        LogWaitingForAgentToStartListening();
        var waitForStartTask = Task.Run(async () =>
        {
            while (!ShouldStopWaiting(isServiceListening, forcefulShutdown, cancellationToken))
            {
                isServiceListening = IsServiceListening(agentExecutablePath, port);

                await Task.Delay(10, cancellationToken);
            }
        }, cancellationToken);
        waitForStartTask.Wait(TimeSpan.FromSeconds(ServiceStartTimeoutInSeconds), cancellationToken);
        return (agentRunnerTask, isServiceListening);
    }

    private bool IsServiceListening(string agentExecutablePath, int port)
    {
        bool isServiceListening;
        LogAttemptingConnection(agentExecutablePath, port);
        var channel = GrpcChannel.ForAddress($"http://localhost:{port}");
        var healthClient = new Health.HealthClient(channel);
        try
        {
            var request = new HealthCheckRequest { Service = Agent.Agent.Descriptor.FullName };
            var response = healthClient.Check(request);
            isServiceListening = response.Status == HealthCheckResponse.Types.ServingStatus.Serving;
        }
        catch (AggregateException error)
        {
            if (!ContainsException<RpcException>(error))
            {
                throw;
            }

            isServiceListening = false;
        }
        catch (RpcException)
        {
            isServiceListening = false;
        }

        if (isServiceListening)
        {
            LogConnectionSuccessful(agentExecutablePath, port);
        }
        else
        {
            LogConnectionFailed(agentExecutablePath, port);
        }

        return isServiceListening;
    }

    private static bool ContainsException<TException>(AggregateException error)
    {
        var isPresentInCurrentLevel = error.InnerExceptions.OfType<TException>().Any();
        if (!isPresentInCurrentLevel)
        {
            return error.InnerExceptions
                .OfType<AggregateException>()
                .Any(ContainsException<TException>);
        }

        return isPresentInCurrentLevel;
    }

    private static bool ShouldStopWaiting(bool isServiceListening, CancellationTokenSource forcefulShutdown,
        CancellationToken cancellationToken)
    {
        return isServiceListening ||
            cancellationToken.IsCancellationRequested ||
            forcefulShutdown.IsCancellationRequested;
    }

    private static async ValueTask WithAcceptableExceptionsAsync(Func<ValueTask> action)
    {
        try
        {
            await action();
        }
        catch (AggregateException error)
        {
            if (!error.InnerExceptions.Any(exception => s_acceptableExceptions.Contains(exception.GetType())))
            {
                // Only rethrow if the exception is not one of the ones that are expected.
                throw;
            }
        }
        catch (Exception error)
        {
            if (!s_acceptableExceptions.Contains(error.GetType()))
            {
                // Only rethrow if the exception is not one of the ones that are expected.
                throw;
            }
        }
    }

    private static void WithAcceptableExceptions(Action action)
    {
        WithAcceptableExceptionsAsync(() =>
        {
            action();
            return ValueTask.CompletedTask;
        }).AsTask().Wait();
    }

    private record struct AgentDescriptor(
        IAgentReader AgentReader,
        Task AgentServiceRunner,
        CancellationTokenSource ForcefulShutdown
    );

    private List<AgentDescriptor> GetAllAgentDescriptors()
    {
        var result = new List<AgentDescriptor>();

        foreach (var agents in _agentsByExecutable.Values)
        {
            result.AddRange(agents);
        }

        return result;
    }

    public void Dispose()
    {
        LogDisposeStarting();

        LogSignallingForcefulStop();
        foreach (var agentDescriptor in GetAllAgentDescriptors())
        {
            // TODO: ForcefulShutdown should never be null, but it sometimes is. That's why the null
            // check is here, even though the semantics of the type being nullable means that it
            // shouldn't ever be null. We need to investigate where the null value is coming from
            // and then either preventing it, or marking the type as nullable.
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            agentDescriptor.ForcefulShutdown?.Cancel();
        }

        LogWaitingForRunnersToStop();
        try
        {
            var tasks = GetAllAgentDescriptors()
                .Select(agent => agent.AgentServiceRunner)
                // TODO: This value _shouldn't_ ever be null, but for some reason there is sometimes
                // at least one null in the list of agent descriptors. I'm not sure what's causing
                // that, so I'm disabling the warning for now. It's probably a good idea to figure
                // out why the null value is showing up, and solve the problem there instead.
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                .Where(value => value != null)
                .ToArray();
            Task.WaitAll(tasks);
        }
        catch (TaskCanceledException)
        {
            // task cancellation is expected
        }
        catch (AggregateException error)
        {
            if (!error.InnerExceptions.All(value => value is TaskCanceledException))
            {
                // only rethrow if one of the exceptions is not the expected TaskCancellationException
                throw;
            }
        }

        LogDisposingServiceRunners();
        foreach (var agent in GetAllAgentDescriptors())
        {
            // TODO: AgentServiceRunner should never be null, but it sometimes is. That's why the null
            // check is here, even though the semantics of the type being nullable means that it
            // shouldn't ever be null. We need to investigate where the null value is coming from
            // and then either preventing it, or marking the type as nullable.
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            agent.AgentServiceRunner?.Dispose();

            // TODO: ForcefulShutdown should never be null, but it sometimes is. That's why the null
            // check is here, even though the semantics of the type being nullable means that it
            // shouldn't ever be null. We need to investigate where the null value is coming from
            // and then either preventing it, or marking the type as nullable.
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            agent.ForcefulShutdown?.Dispose();
        }

        LogDisposeComplete();

        GC.SuppressFinalize(this);
    }

    private void LogAgentQueueInUnexpectedState() => _logger.LogDebug("The agents queue is in an unexpected state");

    private void LogAttemptingConnection(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Attempting to connect to {Agent} health service on port {Port}",
            agentExecutablePath,
            port
        );

    private void LogAttemptingToStart() => _logger.LogDebug("Attempting to start an agent service");

    private void LogConnectingToGrpcService(int port) => _logger.LogDebug("Connecting to gRPC service on port {Port}", port);

    private void LogConnectionFailed(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Failed to connect to {Agent} health service on port {Port}. Trying again.",
            agentExecutablePath,
            port
        );

    private void LogConnectionSuccessful(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Successful connection to {Agent} health service on port {Port}",
            agentExecutablePath,
            port
        );

    private void LogDisposeComplete() => _logger.LogDebug("Dispose complete");

    private void LogDisposeStarting() => _logger.LogDebug("Dispose starting...");

    private void LogDisposingServiceRunners() => _logger.LogDebug("Disposing service runner tasks");

    private void LogFailedToStart() => _logger.LogDebug("Failed to start agent service");

    private void LogFailedToStart(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Failed to start agent {Agent} on port {Port}. Trying again.",
            agentExecutablePath,
            port
        );

    private void LogForcefullyStopping() => _logger.LogDebug("Stopping agent service runner (forcefully)");

    private void LogGettingAgentReader(string agentExecutablePath) => _logger.LogDebug("Getting reader for {AgentExe}", agentExecutablePath);

    private void LogGivingUpBecauseOfCancellationRequest() => _logger.LogDebug("Giving up because cancellation has been requested");

    private void LogNewAgentReader(AgentDescriptor agent) => _logger.LogDebug("Returning new agent reader {hash}", agent.AgentReader.GetHashCode());

    private void LogReceivedCommand(CommandEvent commandEvent) => _logger.LogDebug("Received command event {@Event}", commandEvent);

    private void LogRetrievingExistingAgentReader(AgentDescriptor agent) => _logger.LogDebug("Retrieving an existing agent reader {hash}", agent.AgentReader.GetHashCode());

    private void LogSignallingForcefulStop() => _logger.LogDebug("Signaling service runner tasks to stop (forcefully)");

    private void LogStartingAgent(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Starting agent {Agent} service on port {Port}",
            agentExecutablePath,
            port
        );

    private void LogStartingNewAgentService() => _logger.LogDebug("Starting a new agent service");

    private void LogWaitingForAgentToStartListening() => _logger.LogDebug("Waiting for agent service to start listening");

    private void LogWaitingForRunnersToStop() => _logger.LogDebug("Waiting for service runner tasks to stop");
}
