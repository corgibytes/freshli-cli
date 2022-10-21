using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentManager : IAgentManager
{
    private readonly ICommandInvoker _commandInvoker;

    public AgentManager(ICommandInvoker commandInvoker) => _commandInvoker = commandInvoker;

    public IAgentReader GetReader(string agentExecutablePath) => new AgentReader(_commandInvoker, agentExecutablePath);
}
