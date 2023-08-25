using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Services;

internal record struct AgentDescriptor(
    IAgentReader AgentReader,
    Task AgentServiceRunner,
    CancellationTokenSource ForcefulShutdown
);
