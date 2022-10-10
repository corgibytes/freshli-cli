using System;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class LibYearComputedForPackageEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public IHistoryStopData HistoryStopData { get; init; }
    public string AgentExecutablePath { get; init; }
    public PackageLibYear PackageLibYear { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) => throw new System.NotImplementedException();
}
