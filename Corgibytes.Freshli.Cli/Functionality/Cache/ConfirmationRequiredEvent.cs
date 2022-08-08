using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class ConfirmationRequiredEvent : IApplicationEvent
{
    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
