using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

internal record struct AgentDescriptor(
    IAgentReader AgentReader,
    Task AgentServiceRunner,
    CancellationTokenSource ForcefulShutdown
);
