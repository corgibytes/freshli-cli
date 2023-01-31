using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

// ReSharper disable once UnusedType.Global
public class LibYearComputationForBomStartedEvent : IApplicationEvent
{
    // TODO: Determine if this class is used, since it's just throwing a not implemented exception
    public ValueTask Handle(IApplicationActivityEngine eventClient) => throw new NotImplementedException();
}
