using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CliWrap.EventStream;
using CliWrap.Exceptions;
using Corgibytes.Freshli.Cli.Functionality;
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
        _logger.LogDebug("Getting reader for {AgentExe}", agentExecutablePath);
        var agents = GetAgentsFor(agentExecutablePath);

        lock (agents)
        {
            AgentDescriptor agent;
            if (agents.Count < _configuration.AgentServiceCount)
            {
                _logger.LogDebug("Starting a new agent service");
                var startAgentTask = StartAgentServiceOnAvailablePort(agentExecutablePath, cancellationToken);
                WithAcceptableExceptions(() => startAgentTask.Wait(cancellationToken));

                if (startAgentTask.Status != TaskStatus.RanToCompletion)
                {
                    _logger.LogDebug("Failed to start agent service");
                    throw new AgentRetrievalFailure();
                }

                agent = startAgentTask.Result;
                agents.Enqueue(agent);

                _logger.LogDebug("Returning new agent reader {hash}", agent.AgentReader.GetHashCode());
                return agent.AgentReader;
            }

            if (!agents.TryDequeue(out agent))
            {
                _logger.LogDebug("The agents queue is in an unexpected state");
                throw new AgentRetrievalFailure();
            }

            _logger.LogDebug("Retrieving an existing agent reader {hash}", agent.AgentReader.GetHashCode());
            agents.Enqueue(agent);
            return agent.AgentReader;
        }
    }

    private async Task<AgentDescriptor> StartAgentServiceOnAvailablePort(string agentExecutablePath, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            _logger.LogDebug("Attempting to start an agent service");

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Giving up because cancellation has been requested");
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

            _logger.LogDebug(
                "Failed to start agent {Agent} on port {Port}. Trying again.",
                agentExecutablePath,
                port
            );

            await ForcefullyShutdownAgentRunner(forcefulShutdown, agentRunnerTask);
        }
    }

    private async Task ForcefullyShutdownAgentRunner(CancellationTokenSource forcefulShutdown, Task agentRunnerTask)
    {
        _logger.LogDebug("Stopping agent service runner (forcefully)");
        forcefulShutdown.Cancel();
        await WithAcceptableExceptionsAsync(async () => await agentRunnerTask);
        forcefulShutdown.Dispose();
    }

    private AgentReader CreateAgentReader(int port)
    {
        _logger.LogDebug("Connecting to gRPC service on port {Port}", port);
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
        var listeningExpression = new Regex($"[L|l]istening on(:)? (http://0\\.0\\.0\\.0:)?{port}");
        var isServiceListening = false;

        var command = CliWrap.Cli.Wrap(agentExecutablePath).WithArguments(new List<string>
        {
            "start-server",
            port.ToString()
        });

        _logger.LogDebug(
            "Starting agent {Agent} service on port {Port}",
            agentExecutablePath,
            port
        );
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
                _logger.LogDebug("Received command event {@Event}", commandEvent);
                switch (commandEvent)
                {
                    case StandardOutputCommandEvent output:
                        if (!isServiceListening)
                        {
                            isServiceListening = listeningExpression.IsMatch(output.Text);
                            if (isServiceListening)
                            {
                                _logger.LogDebug(
                                    "Agent {Agent} is listening on port {Port}",
                                    agentExecutablePath,
                                    port
                                );
                            }
                        }

                        break;
                }
            }
        }, cancellationToken);

        _logger.LogDebug("Waiting for agent service to start listening");
        var waitForStartTask = Task.Run(async () =>
        {
            while (!ShouldStopWaiting(isServiceListening, forcefulShutdown, cancellationToken))
            {
                await Task.Delay(10, cancellationToken);
            }
        }, cancellationToken);
        waitForStartTask.Wait(TimeSpan.FromSeconds(ServiceStartTimeoutInSeconds), cancellationToken);
        return (agentRunnerTask, isServiceListening);
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
        _logger.LogDebug("Dispose starting...");

        _logger.LogDebug("Signaling service runner tasks to stop (forcefully)");
        foreach (var agentDescriptor in GetAllAgentDescriptors())
        {
            agentDescriptor.ForcefulShutdown?.Cancel();
        }

        _logger.LogDebug("Waiting for service runner tasks to stop");
        try
        {
            var tasks = GetAllAgentDescriptors()
                .Select(agent => agent.AgentServiceRunner)
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

        _logger.LogDebug("Disposing service runner tasks");
        foreach (var agent in GetAllAgentDescriptors())
        {
            agent.AgentServiceRunner?.Dispose();
            agent.ForcefulShutdown?.Dispose();
        }

        _logger.LogDebug("Dispose complete");

        GC.SuppressFinalize(this);
    }
}
