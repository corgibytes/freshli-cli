using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ThrowExceptionActivity: IApplicationActivity
{
    public void Handle(IApplicationEventEngine eventClient)
    {
        throw new Exception("Simulating failure from an activity");
    }
}
