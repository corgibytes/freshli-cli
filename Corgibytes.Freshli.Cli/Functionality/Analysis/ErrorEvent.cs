using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public abstract class ErrorEvent : ApplicationEventBase
{
    public string ErrorMessage { get; init; } = null!;
    public Exception? Exception { get; init; }

    public override ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        return ValueTask.CompletedTask;
    }
}
