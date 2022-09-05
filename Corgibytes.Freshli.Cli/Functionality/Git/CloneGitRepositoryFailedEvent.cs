using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CloneGitRepositoryFailedEvent : IApplicationEvent
{
    public string ErrorMessage { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
