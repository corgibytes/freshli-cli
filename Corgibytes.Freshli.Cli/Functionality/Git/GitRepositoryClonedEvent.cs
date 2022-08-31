using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryClonedEvent : IApplicationEvent
{
    public string GitRepositoryId { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
