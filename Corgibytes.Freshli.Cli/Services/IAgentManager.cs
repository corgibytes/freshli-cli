using System.Threading;

namespace Corgibytes.Freshli.Cli.Services;

public interface IAgentManager
{
    public IAgentReader GetReader(string agentExecutablePath, CancellationToken cancellationToken = default);
}
