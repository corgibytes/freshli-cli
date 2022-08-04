using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public abstract class FailureEvent : IApplicationEvent
{
    public string ErrorMessage { get; init; } = null!;

    public virtual void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
