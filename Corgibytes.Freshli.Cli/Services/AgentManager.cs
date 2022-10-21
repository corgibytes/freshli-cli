using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentManager : IAgentManager
{
    private readonly ICacheManager _cacheManager;
    private readonly ICommandInvoker _commandInvoker;

    public AgentManager(ICacheManager cacheManager, ICommandInvoker commandInvoker)
    {
        _cacheManager = cacheManager;
        _commandInvoker = commandInvoker;
    }

    public IAgentReader GetReader(string agentExecutablePath) =>
        new AgentReader(_cacheManager, _commandInvoker, agentExecutablePath);
}
