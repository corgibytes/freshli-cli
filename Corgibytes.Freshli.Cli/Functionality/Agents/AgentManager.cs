using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap.EventStream;
using CliWrap.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using Corgibytes.Freshli.Cli.Functionality.Support;
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public class AgentManager : IAgentManager, IDisposable
{
    // TODO: Make this a configurable value
    private const int ServiceStartTimeoutInSeconds = 10;

    private readonly ICacheManager _cacheManager;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<AgentDescriptor>> _agentsByExecutable = new();

    private readonly AgentManagerLogger _logger;
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
        _logger = new AgentManagerLogger(logger);
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    private ConcurrentQueue<AgentDescriptor> GetAgentsFor(string agentExecutablePath)
    {
        return _agentsByExecutable.GetOrAdd(agentExecutablePath, new ConcurrentQueue<AgentDescriptor>());
    }

    public IAgentReader GetReader(string agentExecutablePath, CancellationToken cancellationToken = default)
    {
        _logger.LogGettingAgentReader(agentExecutablePath);
        var agents = GetAgentsFor(agentExecutablePath);

        lock (agents)
        {
            AgentDescriptor agent;
            if (agents.Count < _configuration.AgentServiceCount)
            {
                _logger.LogStartingNewAgentService();
                var startAgentTask = StartAgentServiceOnAvailablePort(agentExecutablePath, cancellationToken);
                WithAcceptableExceptions(() => startAgentTask.Wait(cancellationToken));

                if (startAgentTask.Status != TaskStatus.RanToCompletion)
                {
                    _logger.LogFailedToStart();
                    throw new AgentRetrievalFailure();
                }

                agent = startAgentTask.Result;
                agents.Enqueue(agent);

                _logger.LogNewAgentReader(agent);
                return agent.AgentReader;
            }

            if (!agents.TryDequeue(out agent))
            {
                _logger.LogAgentQueueInUnexpectedState();
                throw new AgentRetrievalFailure();
            }

            _logger.LogRetrievingExistingAgentReader(agent);
            agents.Enqueue(agent);
            return agent.AgentReader;
        }
    }

    private async Task<AgentDescriptor> StartAgentServiceOnAvailablePort(string agentExecutablePath, CancellationToken cancellationToken = default)
    {
        var agentName = Path.GetFileName(agentExecutablePath);

        while (true)
        {
            _logger.LogAttemptingToStart();

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogGivingUpBecauseOfCancellationRequest();
                return default;
            }

            var port = _portFinder.FindNext();

            var forcefulShutdown = new CancellationTokenSource();

            var (agentRunnerTask, isServiceListening) = AttemptToStartAgentRunner(
                agentExecutablePath, port, forcefulShutdown, cancellationToken);

            if (isServiceListening)
            {
                var agentReader = CreateAgentReader(agentName, port);
                return new AgentDescriptor(agentReader, agentRunnerTask, forcefulShutdown);
            }

            _logger.LogFailedToStart(agentExecutablePath, port);

            await ForcefullyShutdownAgentRunner(forcefulShutdown, agentRunnerTask);
        }
    }

    public IPackageLibYearCalculator GetLibYearCalculator(IAgentReader reader, PackageURL packageUrl,
        DateTimeOffset asOfDateTime)
    {
        var logger = _serviceProvider.GetService<ILogger<PackageLibYearCalculator>>();
        return new PackageLibYearCalculator(reader, packageUrl, asOfDateTime, logger);
    }

    private async Task ForcefullyShutdownAgentRunner(CancellationTokenSource forcefulShutdown, Task agentRunnerTask)
    {
        _logger.LogForcefullyStopping();
        forcefulShutdown.Cancel();
        await WithAcceptableExceptionsAsync(async () => await agentRunnerTask);
        forcefulShutdown.Dispose();
    }

    private AgentReader CreateAgentReader(string name, int port)
    {
        _logger.LogConnectingToGrpcService(port);
        var channel = GrpcChannel.ForAddress($"http://localhost:{port}");
        var grpcClient = new Agent.Agent.AgentClient(channel);
        var agentReader = new AgentReader(
            name, _cacheManager, grpcClient, _serviceProvider.GetRequiredService<ILogger<AgentReader>>()
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

        _logger.LogStartingAgent(agentExecutablePath, port);
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
                _logger.LogReceivedCommand(commandEvent);
            }
        }, cancellationToken);

        _logger.LogWaitingForAgentToStartListening();
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
        _logger.LogAttemptingConnection(agentExecutablePath, port);
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
            _logger.LogConnectionSuccessful(agentExecutablePath, port);
        }
        else
        {
            _logger.LogConnectionFailed(agentExecutablePath, port);
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
        _logger.LogDisposeStarting();

        _logger.LogSignallingForcefulStop();
        foreach (var agentDescriptor in GetAllAgentDescriptors())
        {
            // TODO: ForcefulShutdown should never be null, but it sometimes is. That's why the null
            // check is here, even though the semantics of the type being nullable means that it
            // shouldn't ever be null. We need to investigate where the null value is coming from
            // and then either preventing it, or marking the type as nullable.
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            agentDescriptor.ForcefulShutdown?.Cancel();
        }

        _logger.LogWaitingForRunnersToStop();
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

        _logger.LogDisposingServiceRunners();
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

        _logger.LogDisposeComplete();

        GC.SuppressFinalize(this);
    }
}
