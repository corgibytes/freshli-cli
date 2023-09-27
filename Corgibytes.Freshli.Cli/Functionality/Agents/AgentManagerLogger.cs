using CliWrap.EventStream;
using Microsoft.Extensions.Logging;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

internal class AgentManagerLogger
{
    private readonly ILogger<AgentManager> _logger;

    public AgentManagerLogger(ILogger<AgentManager> logger)
    {
        _logger = logger;
    }

    public void LogAgentQueueInUnexpectedState() => _logger.LogDebug("The agents queue is in an unexpected state");

    public void LogAttemptingConnection(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Attempting to connect to {Agent} health service on port {Port}",
            agentExecutablePath,
            port
        );

    public void LogAttemptingToStart() => _logger.LogDebug("Attempting to start an agent service");

    public void LogConnectingToGrpcService(int port) =>
        _logger.LogDebug("Connecting to gRPC service on port {Port}", port);

    public void LogConnectionFailed(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Failed to connect to {Agent} health service on port {Port}. Trying again.",
            agentExecutablePath,
            port
        );

    public void LogConnectionSuccessful(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Successful connection to {Agent} health service on port {Port}",
            agentExecutablePath,
            port
        );

    public void LogDisposeComplete() => _logger.LogDebug("Dispose complete");

    public void LogDisposeStarting() => _logger.LogDebug("Dispose starting...");

    public void LogDisposingServiceRunners() => _logger.LogDebug("Disposing service runner tasks");

    public void LogFailedToStart() => _logger.LogDebug("Failed to start agent service");

    public void LogFailedToStart(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Failed to start agent {Agent} on port {Port}. Trying again.",
            agentExecutablePath,
            port
        );

    public void LogForcefullyStopping() => _logger.LogDebug("Stopping agent service runner (forcefully)");

    public void LogGettingAgentReader(string agentExecutablePath) =>
        _logger.LogTrace("Getting reader for {AgentExe}", agentExecutablePath);

    public void LogGivingUpBecauseOfCancellationRequest() =>
        _logger.LogTrace("Giving up because cancellation has been requested");

    public void LogNewAgentReader(AgentDescriptor agent) =>
        _logger.LogTrace("Returning new agent reader {hash}", agent.AgentReader.GetHashCode());

    public void LogReceivedCommand(CommandEvent commandEvent) =>
        _logger.LogDebug("Received command event {@Event}", commandEvent);

    public void LogRetrievingExistingAgentReader(AgentDescriptor agent) =>
        _logger.LogTrace("Retrieving an existing agent reader {hash}", agent.AgentReader.GetHashCode());

    public void LogSignallingForcefulStop() => _logger.LogDebug("Signaling service runner tasks to stop (forcefully)");

    public void LogStartingAgent(string agentExecutablePath, int port) =>
        _logger.LogDebug(
            "Starting agent {Agent} service on port {Port}",
            agentExecutablePath,
            port
        );

    public void LogStartingNewAgentService() => _logger.LogDebug("Starting a new agent service");

    public void LogWaitingForAgentToStartListening() =>
        _logger.LogDebug("Waiting for agent service to start listening");

    public void LogWaitingForRunnersToStop() => _logger.LogDebug("Waiting for service runner tasks to stop");
}

