using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class AgentsVerifier
{
    public AgentsVerifier(IEnvironment environment) => Environment = environment;

    public IEnvironment Environment { get; }

    public bool Verify() => true;
}
