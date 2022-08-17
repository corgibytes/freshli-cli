using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public abstract class ErrorEvent : IApplicationEvent
{
    public string ErrorMessage { get; init; } = null!;

    public virtual void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
