using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality;

public class LibYearComputedEvent : IApplicationEvent
{
    public IList<PackageLibYear>? LibYearPackages { get; init; }
    public IHistoryStopData? HistoryStopData { get; init; }

    public void Handle(IApplicationActivityEngine eventClient)
    {
    }
}
