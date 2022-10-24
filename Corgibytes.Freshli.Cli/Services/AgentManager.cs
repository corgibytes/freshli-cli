using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentManager : IAgentManager
{
    private readonly IInvoke _invoke;

    public AgentManager(IInvoke invoke) => _invoke = invoke;

    public IAgentReader GetReader(string agentExecutablePath) => new AgentReader(_invoke, agentExecutablePath);
}
